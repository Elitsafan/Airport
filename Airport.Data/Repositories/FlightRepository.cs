using Airport.Data.Accessories;
using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Airport.Data.Repositories
{
    public class FlightRepository : IFlightRepository
    {
        private readonly AirportContext _context;

        public FlightRepository(AirportContext context) => _context = context;
        public async Task AddFlightAsync(Flight flight) => await _context.Flights.AddAsync(flight);
        public IQueryable<Flight> GetAll() => _context.Flights
            .Include(f => f.Stations)
            .AsQueryable();
        public IQueryable<T> OfType<T>() where T : Flight => _context.Flights.OfType<T>();
        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();
        public async Task UpdateFlightAsync(Flight flight)
        {
            var original = await _context.Flights.FindAsync(flight.FlightId) ?? throw new EntityNotFoundException();
            _context.Flights.Entry(original).CurrentValues.SetValues(flight);
        }
        public void Dispose() => _context?.Dispose();
    }
}
