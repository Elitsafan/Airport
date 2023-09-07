using Airport.Models.Entities;
using Airport.Models.Enums;
using Airport.Models.EventArgs;
using Airport.Models.Interfaces;
using Airport.Services.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;

namespace Airport.Services.Logics
{
    public class StationLogic : IStationLogic
    {
        #region Fields
        private readonly SemaphoreSlim _semaphore;
        private readonly Station _station;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<StationLogic> _logger;
        private IFlightLogic? _flightLogic;
        public event AsyncEventHandler<StationChangedEventArgs>? StationChanged;
        #endregion

        public StationLogic(
            IServiceProvider serviceProvider,
            ILogger<StationLogic> logger,
            Station station)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _semaphore = new SemaphoreSlim(1, 1);
            _station = station;
        }

        #region Properties
        public int StationId => _station.StationId;
        public FlightType? CurrentFlightType => _flightLogic?.Flight.ConvertToFlightType();
        public TimeSpan EstimatedWaitingTime => _station.EstimatedWaitingTime;
        public Guid? CurrentFlightId => _flightLogic?.Flight.FlightId;
        public WaitHandle AvailableWaitHandle => _semaphore.AvailableWaitHandle;
        #endregion

        public async Task<IStationLogic> SetFlightAsync(
            IFlightLogic flightLogic,
            CancellationTokenSource? source = null)
        {
            await _semaphore.WaitAsync(source is null ? default : source.Token);
            try
            {
                await flightLogic.ThrowIfCancellationRequested(source);
                _flightLogic = flightLogic;
                //await Console.Out.WriteLineAsync($"{flightLogic.Flight.FlightId} | {flightLogic.Flight.ConvertToFlightType()} | {StationId}");
                if (flightLogic.CurrentStation is not null)
                    await flightLogic.ExitCurrentStationAsync();
                _station.Entrance = DateTime.Now;
                using IStationRepository stationRepository = await UpdateStationAsync();
                await RaiseStationChangedAsync(flightLogic.Flight);
            }
            catch (OperationCanceledException)
            {
                _semaphore.Release();
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{flightLogic.Flight.FlightId} | Station: {StationId}");
                throw;
            }
            return this;
        }

        public async Task TakeOutFlightAsync()
        {
            if (_flightLogic == null)
            {
                await Task.FromException(new InvalidOperationException("Station has no flight set"));
                return;
            }
            try
            {
                await ExitStationAsync();
                await RaiseStationChangedAsync(null);
            }
            catch (Exception e) { await Task.FromException(e); }
        }
        public void Dispose() => _semaphore?.Dispose();
        protected virtual Task RaiseStationChangedAsync(Flight? flight)
        {
            try
            {
                return StationChanged is null
                    ? Task.CompletedTask
                    : StationChanged.InvokeAsync(this, new StationChangedEventArgs(flight));
            }
            catch (Exception e) { return Task.FromException(e); }
        }
        private async Task ExitStationAsync()
        {
            try
            {
                _station.Exit = DateTime.Now;
                using IStationRepository stationRepository = await UpdateStationAsync();
                // Creates a station-flight model
                var stationFlight = new StationFlight
                {
                    FlightId = _flightLogic!.Flight.FlightId,
                    StationId = StationId,
                    Entrance = _station.Entrance!.Value,
                    Exit = _station.Exit!.Value
                };
                using var stationFlightRepository = await InsertStationFlightAsync(stationFlight);
                // Sets flight logic to null as exiting from the station
                _flightLogic = null;
            }
            catch (Exception e) { await Task.FromException(e); }
            finally { _semaphore.Release(); }
        }
        private async Task<IStationRepository> UpdateStationAsync()
        {
            var stationRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IStationRepository>();
            await stationRepository.UpdateStationAsync(_station);
            await stationRepository.SaveChangesAsync();
            return stationRepository;
        }
        private async Task<IStationFlightRepository> InsertStationFlightAsync(StationFlight stationFlight)
        {
            var stationFlightRepository = _serviceProvider
                .CreateAsyncScope()
                .ServiceProvider
                .GetRequiredService<IStationFlightRepository>();
            await stationFlightRepository.AddStationFlightAsync(stationFlight);
            await stationFlightRepository.SaveChangesAsync();
            return stationFlightRepository;
        }
    }
}
