using GreenSpec.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GreenSpec.Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Config> Configs => Set<Config>();
    public DbSet<Alert> Alerts => Set<Alert>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Config entity configuration
        modelBuilder.Entity<Config>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TempMax).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.HumidityMax).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
        });

        // Alert entity configuration
        modelBuilder.Entity<Alert>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Type).HasConversion<string>().IsRequired();
            entity.Property(e => e.Value).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.Threshold).HasPrecision(5, 2).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).HasConversion<string>().IsRequired();
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
            entity.Property(e => e.PasswordHash).HasMaxLength(256).IsRequired();
            entity.HasIndex(e => e.Username).IsUnique();
        });
    }
}
