using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class StationConfiguration : IEntityTypeConfiguration<Station>
    {
        public void Configure(EntityTypeBuilder<Station> builder)
        {
            builder.HasKey(e => e.StationId);

            builder.HasData(
                new
                {
                    StationId = 1,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(300),
                },
                new
                {
                    StationId = 2,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(300),
                },
                new
                {
                    StationId = 3,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(300),
                },
                new
                {
                    StationId = 4,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(300),
                },
                new
                {
                    StationId = 5,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(300),
                },
                new
                {
                    StationId = 6,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(900),
                },
                new
                {
                    StationId = 7,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(900),
                },
                new
                {
                    StationId = 8,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(300),
                },
                new
                {
                    StationId = 9,
                    EstimatedWaitingTime = TimeSpan.FromMilliseconds(400),
                });
        }
    }
}