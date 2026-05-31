namespace NovaCast.Consumer.Models.Entities;

public class Tenant
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string ApiKey { get; set; } = default!;
    public string Status { get; set; } = default!;
}