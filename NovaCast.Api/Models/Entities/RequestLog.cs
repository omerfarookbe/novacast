using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Models.Entities;

public class RequestLog
{
    public Guid Id { get; set; }
    public string? ApiKey { get; set; }
    public Guid? TenantId { get; set; }
    public string? NotificationType { get; set; }
    public NotificationChannel? Channel { get; set; }
    public string Status { get; set; } = default!; // accepted | rejected
    public string? RejectionReason { get; set; }
    public string? Payload { get; set; } // JSON string
    public DateTime CreatedAt { get; set; }

    // Navigation properties
    public Tenant? Tenant { get; set; }
}