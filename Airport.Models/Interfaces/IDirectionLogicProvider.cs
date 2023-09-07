namespace Airport.Models.Interfaces
{
    public interface IDirectionLogicProvider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        Task<IEnumerable<IDirectionLogic>> FindByRouteId(int routeId);
        IEnumerable<IStationLogic?> GetStationsByTargetAndRoute(int stationLogicId, int routeId);
    }
}