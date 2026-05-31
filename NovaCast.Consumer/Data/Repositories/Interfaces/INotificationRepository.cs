using NovaCast.Consumer.Models.Entities;

namespace NovaCast.Consumer.Data.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateDeliveredAsync(Guid id, DeliveryResult result, CancellationToken cancellationToken = default);
    Task UpdateFailedAsync(Guid id, string failureReason, CancellationToken cancellationToken = default);
    Task UpdateRetryingAsync(Guid id, int retryCount, CancellationToken cancellationToken = default);
    Task UpdateDeadLetteredAsync(Guid id, string failureReason, CancellationToken cancellationToken = default);
}