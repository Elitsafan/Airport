using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class TrafficLightConfiguration : IEntityTypeConfiguration<TrafficLight>
    {
        public void Configure(EntityTypeBuilder<TrafficLight> builder)
        {
            builder.HasKey(e => e.TrafficLightId);

            builder.HasMany(e => e.Routes)
                .WithMany(e => e.TrafficLights);

            builder.HasOne(e => e.Station)
                .WithOne(e => e.TrafficLight)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasData(
                new TrafficLight
                {
                    TrafficLightId = 1,
                    StationId = 4,
                },
                new TrafficLight
                {
                    TrafficLightId = 2,
                    StationId = 6,
                },
                new TrafficLight
                {
                    TrafficLightId = 3,
                    StationId = 7,
                });

            builder.HasMany(e => e.Routes)
                .WithMany(e => e.TrafficLights)
                .UsingEntity(
                    "RouteTrafficLight",
                    l => l.HasOne(typeof(Route)).WithMany().HasForeignKey("RoutesRouteId").HasPrincipalKey(nameof(Route.RouteId)),
                    r => r.HasOne(typeof(TrafficLight)).WithMany().HasForeignKey("TrafficLightsTrafficLightId").HasPrincipalKey(nameof(TrafficLight.TrafficLightId)),
                    j =>
                    {
                        j.HasKey("RoutesRouteId", "TrafficLightsTrafficLightId");
                        j.HasData(
                            new 
                            {
                                RoutesRouteId = 1,
                                TrafficLightsTrafficLightId = 1,
                            },
                            new 
                            {
                                RoutesRouteId = 2,
                                TrafficLightsTrafficLightId = 1,
                            },
                            new 
                            {
                                RoutesRouteId = 1,
                                TrafficLightsTrafficLightId = 2,
                            },
                            new 
                            {
                                RoutesRouteId = 2,
                                TrafficLightsTrafficLightId = 2,
                            },
                            new 
                            {
                                RoutesRouteId = 1,
                                TrafficLightsTrafficLightId = 3,
                            },
                            new 
                            {
                                RoutesRouteId = 2,
                                TrafficLightsTrafficLightId = 3,
                            });
                    });
        }
    }
}
