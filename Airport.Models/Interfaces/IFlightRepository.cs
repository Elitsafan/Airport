using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IFlightRepository : IRepository<Flight>
    {
        Task AddFlightAsync(Flight flight);
        Task<IEnumerable<T>> OfTypeAsync<T>() where T : Flight;
        Task<bool> UpdateFlightAsync(Flight flight);
    }
}
