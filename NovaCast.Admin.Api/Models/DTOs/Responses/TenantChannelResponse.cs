namespace NovaCast.Admin.Api.Models.DTOs.Responses;

public class TenantChannelResponse
{
    public Guid Id { get; set; }
    public string Channel { get; set; } = default!;
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
}