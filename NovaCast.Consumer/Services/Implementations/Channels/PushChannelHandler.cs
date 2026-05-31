using NovaCast.Consumer.Models.Entities;
using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Implementations.Channels;

public class PushChannelHandler : IChannelHandler
{
    private readonly ILogger<PushChannelHandler> _logger;

    public PushChannelHandler(ILogger<PushChannelHandler> logger)
    {
        _logger = logger;
    }

    public async Task<DeliveryResult> DeliverAsync(
        NotificationEvent notification,
        TenantChannel channel,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // TODO: integrate with Firebase using channel.Credentials
            _logger.LogInformation(
                "Delivering PUSH notification {NotificationId} to {Recipient} via {Provider}",
                notification.Id,
                notification.Recipient,
                "firebase");

            await Task.Delay(100, cancellationToken);

            return DeliveryResult.Succeeded("firebase", Guid.NewGuid().ToString(), "success");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Push delivery failed for notification {NotificationId}", notification.Id);
            return DeliveryResult.Failed(ex.Message);
        }
    }
}