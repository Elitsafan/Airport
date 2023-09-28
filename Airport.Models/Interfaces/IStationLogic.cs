using Airport.Models.Enums;
using Airport.Models.EventArgs;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IStationLogic : IDisposable
    {
        event AsyncEventHandler<StationChangedEventArgs> StationChanged;
        ObjectId StationId { get; }
        /// <summary>
        /// Gets the <see cref="FlightType"/> if there is a flight
        /// </summary>
        FlightType? CurrentFlightType { get; }
        /// <summary>
        /// Gets the time requires to wait on station
        /// </summary>
        TimeSpan EstimatedWaitingTime { get; }
        /// <summary>
        /// Gets the flight id if there is a flight
        /// </summary>
        ObjectId? CurrentFlightId { get; }
        WaitHandle AvailableWaitHandle { get; }
        Task<IStationLogic> SetFlight(
            IFlightLogic flightLogic,
            CancellationTokenSource? source = null);
        /// <summary>
        /// Takes the flight out from the station
        /// </summary>
        /// <returns></returns>
        Task Clear();
    }
}
