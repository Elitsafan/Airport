using Airport.Models.Entities;
using Airport.Models.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Airport.Data.Repositories
{
    public class TrafficLightRepository : ITrafficLightRepository
    {
        private readonly AirportContext _context;

        public TrafficLightRepository(AirportContext airportContext) => _context = airportContext;

        public IQueryable<TrafficLight> GetAll() => _context.TrafficLights
            .Include(tl => tl.Station)
            .Include(tl => tl.Routes!)
                .ThenInclude(r => r.Directions)
            .AsQueryable();
        public void Dispose() => _context?.Dispose();
    }
}
