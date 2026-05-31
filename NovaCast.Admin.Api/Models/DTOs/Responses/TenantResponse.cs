namespace NovaCast.Admin.Api.Models.DTOs.Responses;

public class TenantResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public List<TenantChannelResponse> Channels { get; set; } = new();
}