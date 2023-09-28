using MongoDB.Bson;
using System.Linq.Expressions;

namespace Airport.Models.Interfaces
{
    public interface IStationLogicProvider
    {
        /// <summary>
        /// Finds all the <see cref="IStationLogic"/> belings to the <paramref name="routeId"/>
        /// </summary>
        /// <param name="routeId"></param>
        /// <returns></returns>
        Task<IEnumerable<IStationLogic>> FindByRouteIdAsync(ObjectId routeId);
        IEnumerable<IStationLogic> GetAll();
        IEnumerable<IStationLogic> FindBy(Expression<Func<IStationLogic, bool>> predicate);
        Task<IEnumerable<IStationLogic>> GetStationsByTargetAndRouteAsync(ObjectId stationLogicId, ObjectId routeId);
    }
}
