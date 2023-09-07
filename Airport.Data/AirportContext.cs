using Airport.Data.Configurations;
using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Airport.Data
{
    public class AirportContext : DbContext
    {
        public AirportContext()
            : base()
        {
        }

        public AirportContext(DbContextOptions<AirportContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Flight> Flights { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<Station> Stations { get; set; }
        public virtual DbSet<Direction> Directions { get; set; }
        public virtual DbSet<StationFlight> StationsFlights { get; set; }
        public virtual DbSet<TrafficLight> TrafficLights { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new DepartureConfiguration());
            builder.ApplyConfiguration(new DirectionConfiguration());
            builder.ApplyConfiguration(new FlightConfiguration());
            builder.ApplyConfiguration(new LandingConfiguration());
            builder.ApplyConfiguration(new RouteConfiguration());
            builder.ApplyConfiguration(new StationConfiguration());
            builder.ApplyConfiguration(new StationFlightConfiguration());
            builder.ApplyConfiguration(new TrafficLightConfiguration());
        }
    }
}