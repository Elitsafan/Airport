using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task AddFlightAsync(Flight flight);
        IQueryable<T> OfType<T>() where T : Flight;
        Task<int> SaveChangesAsync();
        Task UpdateFlightAsync(Flight flight);
    }
}
