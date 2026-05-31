namespace NovaCast.Consumer.Models.Entities;

public class TenantChannel
{
    public Guid Id { get; set; }
    public Guid TenantId { get; set; }
    public string Channel { get; set; } = default!;
    public string Credentials { get; set; } = default!;
    public bool IsEnabled { get; set; }
}