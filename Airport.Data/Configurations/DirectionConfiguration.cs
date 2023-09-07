using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class DirectionConfiguration : IEntityTypeConfiguration<Direction>
    {
        public void Configure(EntityTypeBuilder<Direction> builder)
        {
            builder.HasKey(e => e.DirectionId);

            builder.HasOne(e => e.StationFrom)
                .WithMany(e => e.DirectionsFrom)
                .HasForeignKey(e => e.From)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.StationTo)
                .WithMany(e => e.DirectionsTo)
                .HasForeignKey(e => e.To)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(e => e.Route)
                .WithMany(e => e.Directions)
                .HasForeignKey(e => e.RouteId);

            builder.HasData(
                new Direction
                {
                    DirectionId = 1,
                    From = 1,
                    To = 2,
                    RouteId = 1,
                },
                new Direction
                {
                    DirectionId = 2,
                    From = 2,
                    To = 3,
                    RouteId = 1,
                },
                new Direction
                {
                    DirectionId = 3,
                    From = 3,
                    To = 4,
                    RouteId = 1,
                },
                new Direction
                {
                    DirectionId = 4,
                    From = 4,
                    To = 5,
                    RouteId = 1,
                },
                new Direction
                {
                    DirectionId = 5,
                    From = 5,
                    To = 6,
                    RouteId = 1,
                },
                new Direction
                {
                    DirectionId = 6,
                    From = 5,
                    To = 7,
                    RouteId = 1,
                },
                new Direction
                {
                    DirectionId = 7,
                    From = 6,
                    To = 8,
                    RouteId = 2,
                },
                new Direction
                {
                    DirectionId = 8,
                    From = 7,
                    To = 8,
                    RouteId = 2,
                },
                new Direction
                {
                    DirectionId = 9,
                    From = 8,
                    To = 4,
                    RouteId = 2,
                },
                new Direction
                {
                    DirectionId = 10,
                    From = 4,
                    To = 9,
                    RouteId = 2,
                });
        }
    }
}