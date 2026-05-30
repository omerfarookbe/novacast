using NovaCast.Api.Models.Entities;
using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Data.Repositories.Interfaces;

public interface INotificationRepository
{
    Task<Notification> CreateAsync(Notification notification, CancellationToken cancellationToken = default);
    Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid id, NotificationStatus status, CancellationToken cancellationToken = default);
    Task UpdatePublishedAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Notification>> GetByTenantAsync(Guid tenantId, NotificationStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByTenantAsync(Guid tenantId, NotificationStatus? status, CancellationToken cancellationToken = default);
}