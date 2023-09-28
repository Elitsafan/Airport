using Airport.Models.Entities;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<IEnumerable<Station>> GetStationsByRouteIdAsync(ObjectId routeId);
    }
}
