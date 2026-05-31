using NovaCast.Consumer.Models.Entities;
using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Implementations.Channels;

public class EmailChannelHandler : IChannelHandler
{
    private readonly ILogger<EmailChannelHandler> _logger;

    public EmailChannelHandler(ILogger<EmailChannelHandler> logger)
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
            // TODO: integrate with SendGrid or SMTP using channel.Credentials
            // For now we simulate delivery
            _logger.LogInformation(
                "Delivering EMAIL notification {NotificationId} to {Recipient} via {Provider}",
                notification.Id,
                notification.Recipient,
                "sendgrid");

            await Task.Delay(100, cancellationToken); // simulate API call

            return DeliveryResult.Succeeded("sendgrid", Guid.NewGuid().ToString(), "202 Accepted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email delivery failed for notification {NotificationId}", notification.Id);
            return DeliveryResult.Failed(ex.Message);
        }
    }
}