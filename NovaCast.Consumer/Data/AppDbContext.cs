using Microsoft.EntityFrameworkCore;
using NovaCast.Consumer.Models.Entities;

namespace NovaCast.Consumer.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<TenantChannel> TenantChannels => Set<TenantChannel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global UTC convention
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime) ||
                    property.ClrType == typeof(DateTime?))
                {
                    property.SetColumnType("timestamp with time zone");
                }
            }
        }

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("notifications");
        });

        modelBuilder.Entity<TenantChannel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("tenant_channels");
        });
    }
}