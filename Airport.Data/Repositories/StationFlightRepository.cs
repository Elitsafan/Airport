using Airport.Models.Entities;
using Airport.Models.Interfaces;

namespace Airport.Data.Repositories
{
    public class StationFlightRepository : IStationFlightRepository
    {
        private readonly AirportContext _context;

        public StationFlightRepository(AirportContext context) => _context = context;

        public IQueryable<StationFlight> GetAll() => _context.StationsFlights;
        public async Task AddStationFlightAsync(StationFlight stationReport) => await _context
            .StationsFlights
            .AddAsync(stationReport);
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}
