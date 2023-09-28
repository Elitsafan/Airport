using MongoDB.Bson;

namespace Airport.Models.Interfaces
{
    public interface ITrafficLightLogicProvider
    {
        /// <summary>
        /// Finds all the <see cref="ITrafficLightLogic"/> that associates with the <paramref name="routeId"/>
        /// </summary>
        /// <param name="routeId"></param>
        /// <returns></returns>
        Task<IEnumerable<ITrafficLightLogic>> FindByRouteIdAsync(ObjectId routeId);
    }
}