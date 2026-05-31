using NovaCast.Consumer.Models.Entities;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Interfaces.Channels;

public interface IChannelHandler
{
    Task<DeliveryResult> DeliverAsync(NotificationEvent notification, TenantChannel channel, CancellationToken cancellationToken = default);
}