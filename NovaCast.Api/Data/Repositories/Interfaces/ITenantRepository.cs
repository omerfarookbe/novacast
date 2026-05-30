using NovaCast.Api.Models.Entities;

namespace NovaCast.Api.Data.Repositories.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantChannel?> GetChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default);
    Task<TenantNotificationType?> GetNotificationTypeAsync(Guid tenantId, string type, CancellationToken cancellationToken = default);
}