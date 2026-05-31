using Microsoft.EntityFrameworkCore;
using NovaCast.Admin.Api.Models.Entities;

namespace NovaCast.Admin.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
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

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("tenants");
            entity.HasIndex(e => e.ApiKey).IsUnique();
        });

        modelBuilder.Entity<TenantChannel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("tenant_channels");
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Channels)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}