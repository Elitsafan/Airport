using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class FlightConfiguration : IEntityTypeConfiguration<Flight>
    {
        public void Configure(EntityTypeBuilder<Flight> builder)
        {
            builder.HasKey(e => e.FlightId);
            builder.UseTpcMappingStrategy();

            builder.HasMany(e => e.Stations)
                .WithMany(e => e.Flights)
                .UsingEntity<StationFlight>(
                l => l.HasOne<Station>().WithMany().HasForeignKey(e => e.StationId),
                r => r.HasOne<Flight>().WithMany().HasForeignKey(e => e.FlightId));
        }
    }
}
