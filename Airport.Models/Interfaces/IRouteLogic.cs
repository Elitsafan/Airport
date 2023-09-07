using Microsoft.VisualStudio.Threading;

namespace Airport.Models.Interfaces
{
    public interface IRouteLogic : IEnumerable<IStationLogic>//, IDisposable
    {
        int RouteId { get; }
        string RouteName { get; }
        Func<IStationLogic, bool> IsTrafficJam { get; }
        IEnumerable<IStationLogic> GetNextStationsOf(IStationLogic? stationLogic);
        IEnumerable<IStationLogic> GetStartStations();
        bool HasRightOfWay(IStationLogic? source, IStationLogic target);
        Task<AsyncSemaphore.Releaser> StartRunAsync();
        Task<AsyncSemaphore.Releaser> GetRightOfWayAsync(
            IStationLogic? source,
            IStationLogic target,
            CancellationToken token = default);
    }
}
