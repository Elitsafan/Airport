using Airport.Data.Accessories;
using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Airport.Data.Repositories
{
    public class RouteRepository : IRouteRepository
    {
        private readonly AirportContext _context;

        public RouteRepository(AirportContext context) => _context = context;

        public async Task<Route> GetByIdAsync(int id) => await _context.Routes
            .Include(r => r.Directions)!
                .ThenInclude(d => d.StationFrom)
            .Include(r => r.Directions)!
                .ThenInclude(d => d.StationTo)
            .Include(r => r.Flights)
            .FirstOrDefaultAsync(r => r.RouteId == id) ?? throw new EntityNotFoundException();
        public IQueryable<Route> GetAll() => _context.Routes
            .Include(r => r.Directions)!
                .ThenInclude(d => d.StationFrom)
            .Include(r => r.Directions)!
                .ThenInclude(d => d.StationTo)
            .Include(r => r.Flights)
            .AsQueryable();
        public void Dispose() => _context?.Dispose();
    }
}
