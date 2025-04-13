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

            builder.Property(g => g.Age)
                .IsRequired();
                
            // Relationships are defined in the many-to-many relationship in SharedExperienceConfiguration

            builder.HasMany(g => g.Billings)
                .WithOne(b => b.Guest)
                .HasForeignKey(b => b.GuestId);

            builder.HasMany(g => g.SharedExperiences)
                .WithMany(se => se.Guests)
                .UsingEntity(j => j.ToTable("GuestSharedExperience"));
            
        }
    }
}