namespace NovaCast.Admin.Api.Models.DTOs.Requests;

public class AssignChannelRequest
{
    public string Channel { get; set; } = default!; // email | sms | push | webhook
    public string Credentials { get; set; } = "{}"; // default empty JSON
}