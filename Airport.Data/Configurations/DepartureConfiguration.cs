using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class DepartureConfiguration : IEntityTypeConfiguration<Departure>
    {
        public void Configure(EntityTypeBuilder<Departure> builder)
        {
        }
    }
}