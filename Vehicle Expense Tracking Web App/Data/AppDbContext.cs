using Microsoft.EntityFrameworkCore;
using VehicleExpenseTrackingWebApp.Models;

namespace VehicleExpenseTrackingWebApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Arac> Araclar { get; set; }
    public DbSet<Masraf> Masraflar { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Arac>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Plaka).IsUnique();
        });

        modelBuilder.Entity<Masraf>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Arac)
                  .WithMany(a => a.Masraflar)
                  .HasForeignKey(e => e.AracId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
