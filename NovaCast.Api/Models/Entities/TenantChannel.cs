using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Models.Entities;

public class TenantChannel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Credentials { get; set; } = default!; // JSON string, encrypted at rest
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = default!;
}