using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class StationFlightConfiguration : IEntityTypeConfiguration<StationFlight>
    {
        public void Configure(EntityTypeBuilder<StationFlight> builder)
        {
        }
    }
}
