using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IRouteRepository : IRepository<Route>
    {
        Task<Route> GetByIdAsync(int id);
    }
}
