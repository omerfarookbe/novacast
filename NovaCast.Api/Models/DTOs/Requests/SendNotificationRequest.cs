namespace NovaCast.Api.Models.DTOs.Requests;

public class SendNotificationRequest
{
    public string Type { get; set; } = default!;
    public string NotificationType { get; set; } = default!; // email, sms, push, webhook
    public string Recipient { get; set; } = default!;
    public Dictionary<string, object> Payload { get; set; } = new();
}