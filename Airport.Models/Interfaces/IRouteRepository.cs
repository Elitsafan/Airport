using Airport.Models.Entities;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IRouteRepository : IRepository<Route>
    {
        Task<IEnumerable<Route>> GetRoutesByStationIdAsync(ObjectId stationId);
        Task<Route> GetRouteByIdAsync(ObjectId id);
    }
}
