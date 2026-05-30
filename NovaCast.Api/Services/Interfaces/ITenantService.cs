using NovaCast.Api.Models.Entities;

namespace NovaCast.Api.Services.Interfaces;

public interface ITenantService
{
    Task<Tenant?> GetTenantByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default);
    Task<TenantChannel?> GetTenantChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default);
    Task<TenantNotificationType?> GetTenantNotificationTypeAsync(Guid tenantId, string type, CancellationToken cancellationToken = default);
    Task InvalidateTenantCacheAsync(Guid tenantId, CancellationToken cancellationToken = default);
}