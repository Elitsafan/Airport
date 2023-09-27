using Microsoft.VisualStudio.Threading;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IRouteLogic : IEnumerable<IStationLogic>//, IDisposable
    {
        ObjectId RouteId { get; }
        string RouteName { get; }
        IEnumerable<IStationLogic> GetNextStationsOf(IStationLogic stationLogic);
        IEnumerable<IStationLogic> GetStartStations();
        bool HasRightOfWay(IStationLogic? source, IStationLogic target);
        Task<AsyncSemaphore.Releaser> StartRun();
        Task<AsyncSemaphore.Releaser> GetRightOfWay(
            IStationLogic? source,
            IStationLogic target,
            CancellationToken token = default);
    }
}
