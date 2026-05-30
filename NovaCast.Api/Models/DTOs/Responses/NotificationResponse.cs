namespace NovaCast.Api.Models.DTOs.Responses;

public class NotificationResponse
{
    public Guid Id { get; set; }
    public string Type { get; set; } = default!;
    public string Channel { get; set; } = default!;
    public string Recipient { get; set; } = default!;
    public string Status { get; set; } = default!;
    public int RetryCount { get; set; }
    public string? Provider { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? PublishedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public DateTime? LastRetriedAt { get; set; }
}