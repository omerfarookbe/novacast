namespace NovaCast.Admin.Api.Models.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string Status { get; set; } = default!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public ICollection<TenantChannel> Channels { get; set; } = new List<TenantChannel>();
}