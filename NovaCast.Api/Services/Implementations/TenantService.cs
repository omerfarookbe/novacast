using System.Text.Json;
using NovaCast.Api.Data.Repositories.Interfaces;
using NovaCast.Api.Models.Entities;
using NovaCast.Api.Services.Interfaces;

namespace NovaCast.Api.Services.Implementations;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<TenantService> _logger;

    private static string TenantCacheKey(string apiKey) =>
        $"novacast:tenant:apikey:{apiKey}";

    private static string ChannelCacheKey(Guid tenantId, string channel) =>
        $"novacast:tenant:{tenantId}:channel:{channel}";

    private static string NotificationTypeCacheKey(Guid tenantId, string type) =>
        $"novacast:tenant:{tenantId}:type:{type}";

    public TenantService(
        ITenantRepository tenantRepository,
        ICacheService cacheService,
        ILogger<TenantService> logger)
    {
        _tenantRepository = tenantRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Tenant?> GetTenantByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        var cacheKey = TenantCacheKey(apiKey);

        var cached = await _cacheService.GetAsync<Tenant>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogDebug("Tenant cache hit for api key");
            return cached;
        }

        var tenant = await _tenantRepository.GetByApiKeyAsync(apiKey, cancellationToken);
        if (tenant is not null)
            await _cacheService.SetAsync(cacheKey, tenant, null, cancellationToken);

        return tenant;
    }

    public async Task<TenantChannel?> GetTenantChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default)
    {
        var cacheKey = ChannelCacheKey(tenantId, channel);

        var cached = await _cacheService.GetAsync<TenantChannel>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogDebug("Channel cache hit for tenant {TenantId} channel {Channel}", tenantId, channel);
            return cached;
        }

        var tenantChannel = await _tenantRepository.GetChannelAsync(tenantId, channel, cancellationToken);
        if (tenantChannel is not null)
            await _cacheService.SetAsync(cacheKey, tenantChannel, null, cancellationToken);

        return tenantChannel;
    }

    public async Task<TenantNotificationType?> GetTenantNotificationTypeAsync(Guid tenantId, string type, CancellationToken cancellationToken = default)
    {
        var cacheKey = NotificationTypeCacheKey(tenantId, type);

        var cached = await _cacheService.GetAsync<TenantNotificationType>(cacheKey, cancellationToken);
        if (cached is not null)
        {
            _logger.LogDebug("Notification type cache hit for tenant {TenantId} type {Type}", tenantId, type);
            return cached;
        }

        var notificationType = await _tenantRepository.GetNotificationTypeAsync(tenantId, type, cancellationToken);
        if (notificationType is not null)
            await _cacheService.SetAsync(cacheKey, notificationType, null, cancellationToken);

        return notificationType;
    }

    public async Task InvalidateTenantCacheAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Invalidating cache for tenant {TenantId}", tenantId);
        await _cacheService.RemoveAsync(TenantCacheKey(tenantId.ToString()), cancellationToken);
    }
}