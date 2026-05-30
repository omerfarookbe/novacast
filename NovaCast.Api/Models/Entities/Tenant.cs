using NovaCast.Admin.Api.Models.Entities;
using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Models.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public TenantStatus Status { get; set; } = TenantStatus.Active;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties
    public ICollection<TenantChannel> Channels { get; set; } = new List<TenantChannel>();
    public ICollection<TenantNotificationType> NotificationTypes { get; set; } = new List<TenantNotificationType>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<RequestLog> RequestLogs { get; set; } = new List<RequestLog>();
}