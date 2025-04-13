using ExperienceService.Models;
using ExperienceService.Data.EntityTypeConfigurations;
using Microsoft.EntityFrameworkCore;

namespace ExperienceService.Data
{
    public class SharedExperiencesDbContext : DbContext
    {
        public SharedExperiencesDbContext(DbContextOptions<SharedExperiencesDbContext> options)
            : base(options)
        {
        }
        
        public DbSet<Provider> Providers { get; set; }
        public DbSet<Guest> Guests { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<SharedExperience> SharedExperiences { get; set; }
        public DbSet<Billing> Billings { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Apply configurations
            modelBuilder.ApplyConfiguration(new ProviderConfiguration());
            modelBuilder.ApplyConfiguration(new GuestConfiguration());
            modelBuilder.ApplyConfiguration(new ServiceConfiguration());
            modelBuilder.ApplyConfiguration(new SharedExperienceConfiguration());
            modelBuilder.ApplyConfiguration(new BillingConfiguration());
            modelBuilder.ApplyConfiguration(new DiscountConfiguration());
        }
    }
}