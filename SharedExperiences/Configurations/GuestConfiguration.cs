using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperienceService.Data.EntityTypeConfigurations
{
    public class GuestConfiguration : IEntityTypeConfiguration<Guest>
    {
        public void Configure(EntityTypeBuilder<Guest> builder)
        {
            builder.HasKey(g => g.GuestId);
            
            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(g => g.Number)
                .IsRequired()
                .HasMaxLength(20);
                
            // Relationships are defined in the many-to-many relationship in SharedExperienceConfiguration
        }
    }
}