using NovaCast.Contracts.Events;

namespace NovaCast.Api.Services.Interfaces;

public interface IKafkaProducerService
{
    Task<bool> PublishAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken = default);
}