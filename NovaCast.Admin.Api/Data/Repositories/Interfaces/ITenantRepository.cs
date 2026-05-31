using NovaCast.Admin.Api.Models.Entities;

namespace NovaCast.Admin.Api.Data.Repositories.Interfaces;

public interface ITenantRepository
{
    Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default);
    Task<TenantChannel?> GetChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default);
    Task<TenantChannel> AssignChannelAsync(TenantChannel channel, CancellationToken cancellationToken = default);
    Task RemoveChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default);
}