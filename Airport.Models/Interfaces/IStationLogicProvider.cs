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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "VSTHRD200:Use \"Async\" suffix for async methods", Justification = "<Pending>")]
        Task<IEnumerable<IStationLogic>> FindByRouteId(int routeId);
        IQueryable<IStationLogic> GetAll();
        IStationLogic? FindById(int id);
        IQueryable<IStationLogic> FindBy(Expression<Func<IStationLogic, bool>> predicate);
    }
}
