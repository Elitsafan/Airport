using Airport.Models.Entities;
using Airport.Models.EventArgs;
using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IFlightLogic : IDisposable
    {
        /// <summary>
        /// Occures when the <see cref="Flight"/> is done running
        /// </summary>
        event AsyncEventHandler<FlightRunDoneEventArgs>? FlightRunDone;
        /// <summary>
        /// The <see cref="Entities.Flight"/> that moves across stations
        /// </summary>
        Flight Flight { get; }
        /// <summary>
        /// The current <see cref="IStationLogic"/> that holds the <see cref="Flight"/>
        /// </summary>
        IStationLogic? CurrentStation { get; }
        ObjectId RouteId { get; }
        Task ThrowIfCancellationRequested(CancellationTokenSource? source);
        /// <summary>
        /// Runs the current instance
        /// </summary>
        Task Run();
        void OccupyStation(ObjectId stationId, DateTime time);
        void UnoccupyStation(ObjectId stationId, DateTime time);
    }
}
