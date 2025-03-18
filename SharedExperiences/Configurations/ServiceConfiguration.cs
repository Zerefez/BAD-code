using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperienceService.Data.EntityTypeConfigurations
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            builder.HasKey(s => s.ServiceId);
            
            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(500);
                
            builder.Property(s => s.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            // Relationships
            builder.HasOne(s => s.Provider)
                .WithMany(p => p.Services)
                .HasForeignKey(s => s.ProviderId);
                
            builder.HasOne(s => s.Discount)
                .WithOne(d => d.Service)
                .HasForeignKey<Discount>(d => d.ServiceId);
                
            // Many-to-many relationship with SharedExperience is defined in SharedExperienceConfiguration
        }
    }
}