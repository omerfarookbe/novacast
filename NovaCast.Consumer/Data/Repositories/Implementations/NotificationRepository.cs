using Microsoft.EntityFrameworkCore;
using NovaCast.Consumer.Data.Repositories.Interfaces;
using NovaCast.Consumer.Models.Entities;

namespace NovaCast.Consumer.Data.Repositories.Implementations;

public class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<NotificationRepository> _logger;

    public NotificationRepository(AppDbContext context, ILogger<NotificationRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(n => n.Id == id, cancellationToken);
    }

    public async Task UpdateDeliveredAsync(Guid id, DeliveryResult result, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Status, "Delivered")
                .SetProperty(n => n.Provider, result.Provider)
                .SetProperty(n => n.ProviderMessageId, result.ProviderMessageId)
                .SetProperty(n => n.ProviderResponse, result.ProviderResponse)
                .SetProperty(n => n.ProcessedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task UpdateFailedAsync(Guid id, string failureReason, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Status, "Failed")
                .SetProperty(n => n.FailureReason, failureReason)
                .SetProperty(n => n.ProcessedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task UpdateRetryingAsync(Guid id, int retryCount, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Status, "Retrying")
                .SetProperty(n => n.RetryCount, retryCount)
                .SetProperty(n => n.LastRetriedAt, DateTime.UtcNow),
                cancellationToken);
    }

    public async Task UpdateDeadLetteredAsync(Guid id, string failureReason, CancellationToken cancellationToken = default)
    {
        await _context.Notifications
            .Where(n => n.Id == id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(n => n.Status, "DeadLettered")
                .SetProperty(n => n.FailureReason, failureReason)
                .SetProperty(n => n.ProcessedAt, DateTime.UtcNow),
                cancellationToken);
    }
}