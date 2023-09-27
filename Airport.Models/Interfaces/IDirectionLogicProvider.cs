using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface IDirectionLogicProvider
    {
        Task<IEnumerable<IDirectionLogic>> GetDirectionsByRouteIdAsync(ObjectId routeId);
    }
}