using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperienceService.Data.EntityTypeConfigurations
{
    public class SharedExperienceConfiguration : IEntityTypeConfiguration<SharedExperience>
    {
        public void Configure(EntityTypeBuilder<SharedExperience> builder)
        {
            builder.HasKey(se => se.SharedExperienceId);
            
            builder.Property(se => se.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(se => se.Date)
                .IsRequired();
                
            // Many-to-many relationship with Service
            builder.HasMany(se => se.Services)
                .WithMany(s => s.SharedExperiences)
                .UsingEntity(j => j.ToTable("ServiceSharedExperience"));
                
            // Many-to-many relationship with Guest
            builder.HasMany(se => se.Guests)
                .WithMany(g => g.SharedExperiences)
                .UsingEntity(j => j.ToTable("GuestSharedExperience"));
        }
    }
}