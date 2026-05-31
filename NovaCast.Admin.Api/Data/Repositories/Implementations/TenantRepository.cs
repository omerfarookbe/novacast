using Microsoft.EntityFrameworkCore;
using NovaCast.Admin.Api.Data.Repositories.Interfaces;
using NovaCast.Admin.Api.Models.Entities;

namespace NovaCast.Admin.Api.Data.Repositories.Implementations;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tenant>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Include(t => t.Channels)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tenants
            .AsNoTracking()
            .Include(t => t.Channels)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Tenant> CreateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.CreatedAt = DateTime.UtcNow;
        await _context.Tenants.AddAsync(tenant, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task UpdateAsync(Tenant tenant, CancellationToken cancellationToken = default)
    {
        tenant.UpdatedAt = DateTime.UtcNow;
        _context.Tenants.Update(tenant);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<TenantChannel?> GetChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default)
    {
        return await _context.TenantChannels
            .AsNoTracking()
            .FirstOrDefaultAsync(tc =>
                tc.TenantId == tenantId &&
                tc.Channel == channel,
                cancellationToken);
    }

    public async Task<TenantChannel> AssignChannelAsync(TenantChannel channel, CancellationToken cancellationToken = default)
    {
        channel.CreatedAt = DateTime.UtcNow;
        await _context.TenantChannels.AddAsync(channel, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return channel;
    }

    public async Task RemoveChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default)
    {
        await _context.TenantChannels
            .Where(tc => tc.TenantId == tenantId && tc.Channel == channel)
            .ExecuteDeleteAsync(cancellationToken);
    }
}