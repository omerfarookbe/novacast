using Microsoft.EntityFrameworkCore;
using NovaCast.Api.Data.Repositories.Interfaces;
using NovaCast.Api.Models.Entities;
using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Data.Repositories.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;

    public NotificationRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default)
    {
        notification.CreatedAt = DateTime.UtcNow;
        await _context.Notifications.AddAsync(notification, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return notification;
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, NotificationStatus status, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Status, status)
                .SetProperty(n => n.ProcessedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task UpdatePublishedAsync(Guid id, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Status, NotificationStatus.Published)
                .SetProperty(n => n.PublishedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task<IEnumerable<Notification>> GetByTenantAsync(
        Guid tenantId,
        NotificationStatus? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications
            .AsNoTracking()
            .Where(n => n.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(n => n.Status == status.Value);

        return await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> CountByTenantAsync(
        Guid tenantId,
        NotificationStatus? status,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Notifications
            .Where(n => n.TenantId == tenantId);

        if (status.HasValue)
            query = query.Where(n => n.Status == status.Value);

        return await query.CountAsync(cancellationToken);
    }
}