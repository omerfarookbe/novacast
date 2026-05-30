namespace NovaCast.Api.Models.DTOs.Responses;

public class SendNotificationResponse
{
    public Guid NotificationId { get; set; }
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
}