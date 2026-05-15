using Microsoft.EntityFrameworkCore;
using LogiTechAPI.Models;

namespace LogiTechAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Shipment> Shipments { get; set; } = null!;
        public DbSet<StatusHistory> StatusHistories { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly map models to table names
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Shipment>().ToTable("Shipments");
            modelBuilder.Entity<StatusHistory>().ToTable("StatusHistories");

            // Shipment → StatusHistory ilişkisi.
            // .HasForeignKey("GonderiId") ile FK kolon adı SABİTLENİR — Shipment
            // entity'sine yeniden adlandırılsa bile DB'deki mevcut "GonderiId"
            // kolonu korunur, böylece yeni migration / DB değişikliği gerekmez.
            modelBuilder.Entity<Shipment>()
                .HasMany(g => g.StatusHistory)
                .WithOne()
                .HasForeignKey("GonderiId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
