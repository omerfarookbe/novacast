using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Models.Entities;

public class TenantNotificationType
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Type { get; set; } = default!; // e.g. order.shipped, payment.failed
    public NotificationChannel Channel { get; set; }
    public bool IsEnabled { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = default!;
}