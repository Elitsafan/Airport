using Airport.Models.Entities;

namespace Airport.Models.Interfaces
{
    public interface IStationFlightRepository : IRepository<StationFlight>
    {
        Task AddStationFlightAsync(StationFlight stationReport);
        Task<int> SaveChangesAsync();
    }
}
