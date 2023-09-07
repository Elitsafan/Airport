using Airport.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Airport.Data.Configurations
{
    internal class LandingConfiguration : IEntityTypeConfiguration<Landing>
    {
        public void Configure(EntityTypeBuilder<Landing> builder)
        {
        }
    }
}