using Microsoft.EntityFrameworkCore;
using LogiTechAPI.Models;

namespace LogiTechAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Gonderi> Shipments { get; set; } = null!;
        public DbSet<StatusHistory> StatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Explicitly map models to table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Gonderi>().ToTable("Shipments");
            modelBuilder.Entity<StatusHistory>().ToTable("StatusHistories");

            // Handle the collection property mapping if needed
            modelBuilder.Entity<Gonderi>()
                .HasMany(g => g.StatusHistory)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
