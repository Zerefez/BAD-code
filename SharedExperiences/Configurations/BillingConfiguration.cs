using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperienceService.Data.EntityTypeConfigurations
{
    public class BillingConfiguration : IEntityTypeConfiguration<Billing>
    {
        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder.HasKey(b => b.BillingId);
            
            builder.Property(b => b.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            // Relationships
            builder.HasOne(b => b.Provider)
                .WithMany(p => p.Billings)
                .HasForeignKey(b => b.ProviderId);
                
            builder.HasOne(b => b.Guest)
                .WithMany(g => g.Billings)
                .HasForeignKey(b => b.GuestId);
        }
    }
}