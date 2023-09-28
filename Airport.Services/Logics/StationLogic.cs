using Airport.Models.Entities;
using Airport.Models.Enums;
using Airport.Models.EventArgs;
using Airport.Models.Interfaces;
using Airport.Services.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Services.Logics
{
    public class StationLogic : IStationLogic
    {
        #region Fields
        private readonly SemaphoreSlim _semaphore;
        private readonly Station _station;
        private readonly ILogger<StationLogic> _logger;
        private IFlightLogic? _flightLogic;
        public event AsyncEventHandler<StationChangedEventArgs>? StationChanged;
        #endregion

        public StationLogic(ILogger<StationLogic> logger, Station station)
        {
            _logger = logger;
            _semaphore = new SemaphoreSlim(1, 1);
            _station = station;
        }

        #region Properties
        public ObjectId StationId => _station.StationId;
        public FlightType? CurrentFlightType => _flightLogic?.Flight.ConvertToFlightType();
        public TimeSpan EstimatedWaitingTime => _station.EstimatedWaitingTime;
        public ObjectId? CurrentFlightId => _flightLogic?.Flight.FlightId;
        public WaitHandle AvailableWaitHandle => _semaphore.AvailableWaitHandle;
        #endregion

        public async Task<IStationLogic> SetFlight(
            IFlightLogic flightLogic,
            CancellationTokenSource? source = null)
        {
            await _semaphore.WaitAsync(source is null ? default : source.Token);
            try
            {
                await flightLogic.ThrowIfCancellationRequested(source);
                _flightLogic = flightLogic;
                await Console.Out.WriteLineAsync($"{flightLogic.Flight.FlightId} | {flightLogic.Flight.ConvertToFlightType()} | {StationId}");
                if (flightLogic.CurrentStation is not null)
                    await flightLogic.CurrentStation.Clear();
                _flightLogic.OccupyStation(StationId, DateTime.Now);
                await RaiseStationChanged(flightLogic.Flight);
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
        public async Task Clear()
        {
            if (_flightLogic == null)
            {
                await Task.FromException(new InvalidOperationException("Station has no flight set"));
                return;
            }
            try
            {
                await ClearStation();
                await RaiseStationChanged(null);
            }
            catch (Exception e) { await Task.FromException(e); }
        }
        public void Dispose() => _semaphore?.Dispose();

        protected virtual async Task RaiseStationChanged(Flight? flight)
        {
            try
            {
                var list = StationChanged?.GetInvocationList();
                if (list is null)
                    return;
                var del = list.FirstOrDefault(d => d.Target == _flightLogic);
                await Task.FromResult(del?.DynamicInvoke(new object[] { this, new StationChangedEventArgs(flight) }));
                await Task.WhenAll(list
                    .Where(d => !ReferenceEquals(d, del))
                    .Select(d => Task.FromResult(
                        d?.DynamicInvoke(new object[] { this, new StationChangedEventArgs(flight) }))));
            }
            catch (Exception e) {  await Task.FromException(e); }
        }
        private async Task ClearStation()
        {
            try
            {
                _flightLogic!.UnoccupyStation(_flightLogic.CurrentStation!.StationId, DateTime.Now);
                // Sets flight logic to null as exiting from the station
                _flightLogic = null;
            }
            catch (Exception e) { await Task.FromException(e); }
            finally { _semaphore.Release(); }
        }
    }
}
