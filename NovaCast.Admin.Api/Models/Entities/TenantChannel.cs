using NovaCast.Admin.Api.Models.Entities;

namespace NovaCast.Admin.Api.Models.Entities;

public class TenantChannel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Channel { get; set; } = default!;
    public string Credentials { get; set; } = default!;
    public bool IsEnabled { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public Tenant Tenant { get; set; } = default!;
}