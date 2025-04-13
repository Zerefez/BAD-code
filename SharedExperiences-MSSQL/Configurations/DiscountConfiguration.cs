using ExperienceService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExperienceService.Data.EntityTypeConfigurations
{
    public class DiscountConfiguration : IEntityTypeConfiguration<Discount>
    {
        public void Configure(EntityTypeBuilder<Discount> builder)
        {
            builder.HasKey(d => d.DiscountId);
            
            builder.Property(d => d.DiscountValue)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            builder.Property(d => d.GuestCount)
                .IsRequired();
                
            // Relationship with Service is defined in ServiceConfiguration
        }
    }
}