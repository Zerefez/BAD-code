using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Billing> Billings { get; set; }
    public DbSet<Guest> Guests { get; set; }
    public DbSet<Provider> Providers { get; set; }
    public DbSet<Service> Services { get; set; }
    public DbSet<Discount> Discounts { get; set; }
    public DbSet<SharedExperience> SharedExperiences { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configure many-to-many relationships
        modelBuilder.Entity<Service>()
            .HasMany(s => s.Providers)
            .WithMany(p => p.Services);

        modelBuilder.Entity<Service>()
            .HasMany(s => s.Discounts)
            .WithMany(d => d.Services);

        modelBuilder.Entity<SharedExperience>()
            .HasMany(se => se.Guests)
            .WithMany(g => g.SharedExperiences);

        base.OnModelCreating(modelBuilder);
    }
}
