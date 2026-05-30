using Microsoft.EntityFrameworkCore;
using NovaCast.Api.Data.Repositories.Interfaces;
using NovaCast.Api.Models.Entities;

namespace NovaCast.Api.Data.Repositories.Implementations;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Tenant?> GetByApiKeyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.ApiKey == apiKey, cancellationToken);
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<TenantChannel?> GetChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default)
    {
        return await _context.TenantChannels
            .AsNoTracking()
            .FirstOrDefaultAsync(tc =>
                tc.TenantId == tenantId &&
                tc.Channel.ToString() == channel,
                cancellationToken);
    }

    public async Task<TenantNotificationType?> GetNotificationTypeAsync(Guid tenantId, string type, CancellationToken cancellationToken = default)
    {
        return await _context.TenantNotificationTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(nt =>
                nt.TenantId == tenantId &&
                nt.Type == type,
                cancellationToken);
    }
}