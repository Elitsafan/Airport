using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(e => e.RouteId);

            builder.HasMany(e => e.Flights)
                .WithOne(e => e.Route);

            builder.HasIndex(e => e.RouteName)
                .IsUnique();

            builder.HasData(
                new Route
                {
                    RouteId = 1,
                    RouteName = "Landing",
                },
                new Route
                {
                    RouteId = 2,
                    RouteName = "Departure",
                });
        }
    }
}