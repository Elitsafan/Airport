using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IStationRepository : IRepository<Station>
    {
        Task<int> SaveChangesAsync();
        Task UpdateStationAsync(Station station);
    }
}
