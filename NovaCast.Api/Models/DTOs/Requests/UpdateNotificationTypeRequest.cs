namespace NovaCast.Api.Models.DTOs.Requests;

public class UpdateNotificationTypeRequest
{
    public string Channel { get; set; } = default!;
    public bool IsEnabled { get; set; } = true;
}