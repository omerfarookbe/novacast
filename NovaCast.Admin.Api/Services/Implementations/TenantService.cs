using NovaCast.Admin.Api.Data.Repositories.Interfaces;
using NovaCast.Admin.Api.Models.DTOs.Requests;
using NovaCast.Admin.Api.Models.DTOs.Responses;
using NovaCast.Admin.Api.Models.Entities;
using NovaCast.Admin.Api.Services.Interfaces;

namespace NovaCast.Admin.Api.Services.Implementations;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICacheService _cacheService;
    private readonly ILogger<TenantService> _logger;

    private static string TenantCacheKey(string apiKey) =>
        $"novacast:tenant:apikey:{apiKey}";

    public TenantService(
        ITenantRepository tenantRepository,
        ICacheService cacheService,
        ILogger<TenantService> logger)
    {
        _tenantRepository = tenantRepository;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<List<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return tenants.Select(MapToResponse).ToList();
    }

    public async Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        return tenant is null ? null : MapToResponse(tenant);
    }

    public async Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ApiKey = GenerateApiKey(),
            Status = "Active"
        };

        var created = await _tenantRepository.CreateAsync(tenant, cancellationToken);

        _logger.LogInformation("Tenant {TenantId} created with name {Name}", created.Id, created.Name);

        return MapToResponse(created);
    }

    public async Task<TenantResponse?> UpdateStatusAsync(Guid id, UpdateTenantStatusRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await _tenantRepository.GetByIdAsync(id, cancellationToken);
        if (tenant is null) return null;

        tenant.Status = request.Status;
        await _tenantRepository.UpdateAsync(tenant, cancellationToken);

        // Invalidate cache so API and Consumer pick up the change immediately
        await _cacheService.RemoveAsync(TenantCacheKey(tenant.ApiKey), cancellationToken);

        _logger.LogInformation("Tenant {TenantId} status updated to {Status}", id, request.Status);

        return MapToResponse(tenant);
    }

    public async Task<TenantChannelResponse> AssignChannelAsync(Guid tenantId, AssignChannelRequest request, CancellationToken cancellationToken = default)
    {
        var existing = await _tenantRepository.GetChannelAsync(tenantId, request.Channel, cancellationToken);
        if (existing is not null)
            throw new InvalidOperationException($"Channel '{request.Channel}' is already assigned to this tenant");

        var channel = new TenantChannel
        {
            Id = Guid.NewGuid(),
            TenantId = tenantId,
            Channel = request.Channel,
            Credentials = request.Credentials,
            IsEnabled = true
        };

        var created = await _tenantRepository.AssignChannelAsync(channel, cancellationToken);

        _logger.LogInformation(
            "Channel {Channel} assigned to tenant {TenantId}",
            request.Channel,
            tenantId);

        return MapChannelToResponse(created);
    }

    public async Task RemoveChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default)
    {
        await _tenantRepository.RemoveChannelAsync(tenantId, channel, cancellationToken);

        _logger.LogInformation(
            "Channel {Channel} removed from tenant {TenantId}",
            channel,
            tenantId);
    }

    private static string GenerateApiKey() =>
        $"nvc_live_{Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("=", "").Replace("+", "").Replace("/", "")}";

    private static TenantResponse MapToResponse(Tenant tenant) => new()
    {
        Id = tenant.Id,
        Name = tenant.Name,
        ApiKey = tenant.ApiKey,
        Status = tenant.Status,
        CreatedAt = tenant.CreatedAt,
        Channels = tenant.Channels.Select(MapChannelToResponse).ToList()
    };

    private static TenantChannelResponse MapChannelToResponse(TenantChannel channel) => new()
    {
        Id = channel.Id,
        Channel = channel.Channel,
        IsEnabled = channel.IsEnabled,
        CreatedAt = channel.CreatedAt
    };
}