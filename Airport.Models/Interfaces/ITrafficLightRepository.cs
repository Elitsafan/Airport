using Airport.Models.Entities;
using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface ITrafficLightRepository : IRepository<TrafficLight>
    {
        Task<IEnumerable<TrafficLight>> GetTrafficLightsByRouteIdAsync(ObjectId routeId);
    }
}
