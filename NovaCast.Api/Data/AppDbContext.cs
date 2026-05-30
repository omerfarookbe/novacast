using Microsoft.EntityFrameworkCore;
using NovaCast.Api.Models.Entities;
using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<TenantChannel> TenantChannels => Set<TenantChannel>();
    public DbSet<TenantNotificationType> TenantNotificationTypes => Set<TenantNotificationType>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<RequestLog> RequestLogs => Set<RequestLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Global UTC convention — all DateTime properties stored as timestamptz
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

        // Tenant
        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.ApiKey).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Status)
                  .HasConversion<string>()
                  .HasDefaultValue(TenantStatus.Active);
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.HasIndex(e => e.ApiKey).IsUnique();
        });

        // TenantChannel
        modelBuilder.Entity<TenantChannel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Channel).HasConversion<string>();
            entity.Property(e => e.Credentials).IsRequired();
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.HasIndex(e => new { e.TenantId, e.Channel }).IsUnique();
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Channels)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // TenantNotificationType
        modelBuilder.Entity<TenantNotificationType>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Type).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Channel).HasConversion<string>();
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.HasIndex(e => new { e.TenantId, e.Type }).IsUnique();
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.NotificationTypes)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Notification
        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Channel).HasConversion<string>();
            entity.Property(e => e.Type).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Recipient).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Payload).IsRequired();
            entity.Property(e => e.Status)
                  .HasConversion<string>()
                  .HasDefaultValue(NotificationStatus.Received);
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.Property(e => e.PublishedAt).IsRequired(false);
            entity.Property(e => e.ProcessedAt).IsRequired(false);
            entity.Property(e => e.LastRetriedAt).IsRequired(false);
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => new { e.TenantId, e.Status });
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.Notifications)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // RequestLog
        modelBuilder.Entity<RequestLog>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(e => e.Channel).HasConversion<string>();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
            entity.Property(e => e.CreatedAt)
                  .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasOne(e => e.Tenant)
                  .WithMany(t => t.RequestLogs)
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.SetNull);
        });
    }
}