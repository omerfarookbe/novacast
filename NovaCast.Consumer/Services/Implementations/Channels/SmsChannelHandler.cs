using NovaCast.Consumer.Models.Entities;
using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Implementations.Channels;

public class SmsChannelHandler : IChannelHandler
{
    private readonly ILogger<SmsChannelHandler> _logger;

    public SmsChannelHandler(ILogger<SmsChannelHandler> logger)
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
            // TODO: integrate with Twilio using channel.Credentials
            _logger.LogInformation(
                "Delivering SMS notification {NotificationId} to {Recipient} via {Provider}",
                notification.Id,
                notification.Recipient,
                "twilio");

            await Task.Delay(100, cancellationToken);

            return DeliveryResult.Succeeded("twilio", Guid.NewGuid().ToString(), "queued");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SMS delivery failed for notification {NotificationId}", notification.Id);
            return DeliveryResult.Failed(ex.Message);
        }
    }
}