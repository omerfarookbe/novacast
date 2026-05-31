using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Interfaces;

public interface INotificationDeliveryService
{
    Task ProcessAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken = default);
}