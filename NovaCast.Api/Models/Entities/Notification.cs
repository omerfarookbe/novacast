using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Models.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public NotificationChannel Channel { get; set; }
    public string Type { get; set; } = default!;
    public string Recipient { get; set; } = default!;
    public string Payload { get; set; } = default!; // JSON string
    public NotificationStatus Status { get; set; } = NotificationStatus.Received;
    public int RetryCount { get; set; } = 0;
    public string? Provider { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ProviderResponse { get; set; } // JSON string
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? LastRetriedAt { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = default!;
}