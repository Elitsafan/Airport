using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Airport.Data.Repositories
{
    public class StationRepository : IStationRepository
    {
        private readonly AirportContext _context;
        public StationRepository(AirportContext airportContext) => _context = airportContext;

        public IQueryable<Station> GetAll() => _context.Stations
            .Include(s => s.Flights)
                //.ThenInclude(f => f!.StationReports)
            .Include(s => s.DirectionsFrom)
            .Include(s => s.DirectionsTo)
            //.AsNoTracking()
            .AsQueryable();
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task UpdateStationAsync(Station station)
        {
            if (station is null) throw new ArgumentNullException(nameof(station));
            Station? original = await _context.Stations.FindAsync(station.StationId) ??
                throw new ArgumentException("Station not found");
            _context.Entry(original).CurrentValues.SetValues(station);
        }
        public void Dispose() => _context?.Dispose();
    }
}
