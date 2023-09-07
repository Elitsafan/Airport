using Airport.Models.Entities;
using Airport.Models.EventArgs;
using Airport.Models.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using System.Diagnostics.CodeAnalysis;

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
        private readonly Dictionary<int, AsyncAutoResetEvent> _newFlightGates;
        private AsyncSemaphore.Releaser _rightOfWay;
        public event AsyncEventHandler<FlightRunDoneEventArgs>? FlightRunDone;
        #endregion

        public FlightLogic(
            IFlightRepository repository,
            IRouteLogic routeLogic,
            ILogger<IFlightLogic> logger,
            Flight flight)
        {
            _flightRepository = repository;
            _routeLogic = routeLogic;
            _logger = logger;
            _flight = flight;
            RouteId = routeLogic.RouteId;
            var startStations = _routeLogic
                .GetStartStations()
                .ToList();
            foreach (var station in startStations)
                station.StationChanged += ReigsterFlightAsync;
            // Each start station has its gate
            _newFlightGates = new(
                startStations.Select(
                    s => new KeyValuePair<int, AsyncAutoResetEvent>(s.StationId, new(true))));
            // Opens all the gates
            foreach (var kvp in _newFlightGates)
                kvp.Value.Set();
            _syncEntrance = new AsyncSemaphore(1);
        }

        #region Properties
        public int RouteId { get; }
        public Flight Flight => _flight;
        public IStationLogic CurrentStation { get; private set; } = null!;
        #endregion

        public async Task RunAsync()
        {
            var releaser = await _routeLogic.StartRunAsync();
            try
            {
                // Runs untill attempts to enter a null station
                while (await TryEnterStationAsync(_routeLogic.GetNextStationsOf(CurrentStation)) is not null) ;
                _ = ExitFromLastStationAsync();
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
        public async Task ExitCurrentStationAsync()
        {
            if (CurrentStation is null)
                return;
            await CurrentStation.TakeOutFlightAsync();
        }
        public void Dispose()
        {
            _flightRepository.Dispose();
            _syncEntrance.Dispose();
            _rightOfWay.Dispose();
        }

        private async Task<IStationLogic?> TryEnterStationAsync(IEnumerable<IStationLogic> stations)
        {
            // if there are no stations, there is no entrance
            if (!stations.Any())
                return null;
            try
            {
                bool isNewFlight = CurrentStation is null;
                CurrentStation = await PerformParallelEntrance(stations);
                // No need the right of way anymore
                _rightOfWay.Dispose();
                if (isNewFlight)
                    // Opens the gate to the next flight
                    _newFlightGates[CurrentStation.StationId].Set();
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                throw;
            }
            // Waits 
            await WaitOnStationAsync(CurrentStation!.EstimatedWaitingTime);
            return CurrentStation;
        }
        // Saves the flight to database
        private async Task ReigsterFlightAsync(object? sender, StationChangedEventArgs args)
        {
            if (args.Flight is null || args.Flight != Flight)
                return;
            await _flightRepository.AddFlightAsync(Flight);
            await _flightRepository.SaveChangesAsync();
            foreach (var station in _routeLogic.GetStartStations())
                station.StationChanged -= ReigsterFlightAsync;
        }
        [SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        private async Task<IStationLogic> PerformParallelEntrance(IEnumerable<IStationLogic> stations)
        {
            // An entrance to a only one station does not need a cancellation
            using var cts = stations.Count() == 1 ? null : new CancellationTokenSource();
            var entranceAttempts = stations
                .Select(s => Task.Run(async () => await EnterStationAsync(s, cts)))
                .ToList();
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
        private async Task<IStationLogic> EnterStationAsync(
            IStationLogic target,
            CancellationTokenSource? cts)
        {
            _rightOfWay = await _routeLogic.GetRightOfWayAsync(CurrentStation, target, cts is null ? default : cts.Token);
            // flight is a new one
            if (CurrentStation == null)
                // Waits for the gate to open
                await _newFlightGates[target.StationId].WaitAsync(cts is null ? default : cts.Token);
            try
            {
                return await target.SetFlightAsync(this, cts);
            }
            catch (OperationCanceledException)
            {
                // Opens back the gate if it is a new flight
                if (CurrentStation is null)
                    _newFlightGates[target.StationId].Set();
                _rightOfWay.Dispose();
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, null);
                throw;
            }
        }
        private async Task ExitFromLastStationAsync()
        {
            try
            {
                await ExitCurrentStationAsync();
                await _flightRepository.UpdateFlightAsync(Flight);
                await _flightRepository.SaveChangesAsync();
                if (FlightRunDone is not null)
                    await FlightRunDone.InvokeAsync(this, new FlightRunDoneEventArgs(this));
            }
            catch (Exception e) { await Task.FromException(e); }
        }
        private Task WaitOnStationAsync(TimeSpan time) => Task.Delay(time);
    }
}
