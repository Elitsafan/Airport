using Airport.Models.Entities;
using Airport.Models.EventArgs;
using Airport.Models.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Services.Logics
{
    public class FlightLogic : IFlightLogic
    {
        #region Fields
        private readonly IRouteLogic _routeLogic;
        private readonly ILogger<IFlightLogic> _logger;
        private readonly IFlightRepository _flightRepository;
        private readonly Flight _flight;
        // Used to sync an entrance to a station and cancel the others attempts
        private readonly AsyncSemaphore _syncEntrance;
        // Makes new flights to start the run synchronously
        private readonly Dictionary<ObjectId, AsyncAutoResetEvent> _newFlightGates;
        private AsyncSemaphore.Releaser _rightOfWay;
        public event AsyncEventHandler<FlightRunDoneEventArgs>? FlightRunDone;
        #endregion

        public FlightLogic(
            IFlightRepository repository,
            IRouteLogic routeLogic,
            ILogger<IFlightLogic> logger,
            IAirportHubHandlerRegistrar airportHubHandlerRegistrar,
            Flight flight)
        {
            _flightRepository = repository;
            _routeLogic = routeLogic;
            _logger = logger;
            _flight = flight;
            RouteId = _routeLogic.RouteId;
            var startStations = _routeLogic
                .GetStartStations()
                .ToList();
            foreach (var station in startStations)
                station.StationChanged += RegisterFlight;
            // Each start station has its gate
            _newFlightGates = new(
                startStations.Select(
                    s => new KeyValuePair<ObjectId, AsyncAutoResetEvent>(s.StationId, new(true))));
            // Opens all the gates
            foreach (var kvp in _newFlightGates)
                kvp.Value.Set();
            _syncEntrance = new AsyncSemaphore(1);
            FlightRunDone += airportHubHandlerRegistrar.OnFlightRunDone;
        }

        #region Properties
        public ObjectId RouteId { get; }
        public Flight Flight => _flight;
        public IStationLogic? CurrentStation { get; private set; } = null;
        #endregion

        public async Task Run()
        {
            var releaser = await _routeLogic.StartRun();
            try
            {
                var startStations = _routeLogic.GetStartStations();
                // if there are any stations, tries to enter any
                if (startStations is not null && startStations.Any())
                    await TryEnterStation(startStations);
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                await Task.FromException(e);
            }
            finally { releaser.Dispose(); }
        }
        // Syncs a station entrance and cancel the others
        public async Task ThrowIfCancellationRequested(CancellationTokenSource? source)
        {
            var releaser = await _syncEntrance.EnterAsync(source is null
                ? default
                : source.Token);
            try
            {
                source?.Token.ThrowIfCancellationRequested();
                source?.Cancel(true);
            }
            finally { releaser.Dispose(); }
        }
        public void OccupyStation(ObjectId stationId, DateTime entranceTime)
        {
            if (Flight.StationOccupationDetails.Exists(wd => wd.StationId == stationId))
                throw new InvalidOperationException("Station already occupied");
            Flight.StationOccupationDetails.Add(new StationOccupationDetails
            {
                StationId = stationId,
                Entrance = entranceTime
            });
        }
        public void UnoccupyStation(ObjectId stationId, DateTime exitTime)
        {
            var stationOccupationDetails = Flight.StationOccupationDetails.Find(wd => wd.StationId == stationId)
                ?? throw new InvalidOperationException("Station not found");
            stationOccupationDetails.Exit = exitTime;
        }
        public void Dispose()
        {
            _flightRepository.Dispose();
            _syncEntrance.Dispose();
            _rightOfWay.Dispose();
        }
        // Recursive-like logic of stations entrance
        private async Task TryEnterStation(IEnumerable<IStationLogic> stations)
        {
            try
            {
                bool isNewFlight = CurrentStation is null;
                CurrentStation = await PerformParallelEntranceToStations(stations);
                // No need the right of way anymore
                _rightOfWay.Dispose();
                if (isNewFlight)
                {
                    // Opens the gate to the next flight
                    _newFlightGates[CurrentStation.StationId].Set();
                }
                // Waits 
                await WaitOnStation(CurrentStation.EstimatedWaitingTime);
                var nextStations = _routeLogic.GetNextStationsOf(CurrentStation);
                // Enter to the next stations or the run is done
                _ = nextStations.Any()
                    ? TryEnterStation(nextStations)
                    : EndRun();
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                await Task.FromException(e);
            }
        }
        // Finishes the run
        private async Task EndRun()
        {
            // Exits from the last station
            await CurrentStation!.Clear();
            await _flightRepository.UpdateFlightAsync(Flight);
            if (FlightRunDone is not null)
                await FlightRunDone.InvokeAsync(this, new FlightRunDoneEventArgs(this));
        }
        // Saves the flight to database
        private async Task RegisterFlight(object? sender, StationChangedEventArgs args)
        {
            if (args.Flight is null || args.Flight != Flight)
                return;
            Flight.RouteId = RouteId;
            await _flightRepository.AddFlightAsync(Flight);
            foreach (var station in _routeLogic.GetStartStations())
                station.StationChanged -= RegisterFlight;
        }
        private async Task<IStationLogic> PerformParallelEntranceToStations(IEnumerable<IStationLogic> stations)
        {
            // An entrance to a only one station does not need a cancellation
            using var cts = stations.Count() == 1 ? null : new CancellationTokenSource();
            var entranceAttempts = stations
                .Select(s => Task.Run(async () => await EnterStationAsync(s, cts)))
                .ToList();
            // Filters the attempts until there is a success
            while (entranceAttempts.Count > 0)
            {
                var enteredStation = await Task.WhenAny(entranceAttempts);
                if (enteredStation.IsCompletedSuccessfully)
                    return await enteredStation;
                // Eliminates failures
                if (enteredStation.Status == TaskStatus.Canceled || enteredStation.Status == TaskStatus.Faulted)
                    entranceAttempts.Remove(enteredStation);
            }
            throw new Exception("Couldn't enter any of the stations");
        }

        private async Task<IStationLogic> EnterStationAsync(IStationLogic target, CancellationTokenSource? cts)
        {
            _rightOfWay = await _routeLogic.GetRightOfWay(CurrentStation, target, cts is null ? default : cts.Token);
            // flight is a new one
            bool isNewFlight = CurrentStation == null;
            if (isNewFlight)
                // Waits for the gate to open
                await _newFlightGates[target.StationId].WaitAsync(cts is null ? default : cts.Token);
            try
            {
                return await target.SetFlight(this, cts);
            }
            catch (OperationCanceledException)
            {
                // Opens back the gate if it is a new flight
                if (isNewFlight)
                    _newFlightGates[target.StationId].Set();
                _rightOfWay.Dispose();
                throw;
            }
            catch (ObjectDisposedException)
            {
                // Opens back the gate if it is a new flight
                if (isNewFlight)
                    _newFlightGates[target.StationId].Set();
                _rightOfWay.Dispose();
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                // Opens back the gate if it is a new flight
                if (isNewFlight)
                    _newFlightGates[target.StationId].Set();
                _rightOfWay.Dispose();
                throw;
            }
        }
        private async Task WaitOnStation(TimeSpan time) => await Task.Delay(time);
    }
}
