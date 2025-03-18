using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperienceService.Data.EntityTypeConfigurations
{
    public class ProviderConfiguration : IEntityTypeConfiguration<Provider>
    {
        public void Configure(EntityTypeBuilder<Provider> builder)
        {
            builder.HasKey(p => p.ProviderId);
            
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(p => p.Address)
                .IsRequired()
                .HasMaxLength(200);
                
            builder.Property(p => p.Number)
                .IsRequired()
                .HasMaxLength(20);
                
            builder.Property(p => p.CVR)
                .HasMaxLength(20);
                
            // Relationships
            builder.HasMany(p => p.Services)
                .WithOne(s => s.Provider)
                .HasForeignKey(s => s.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
                
            builder.HasMany(p => p.Billings)
                .WithOne(b => b.Provider)
                .HasForeignKey(b => b.ProviderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}