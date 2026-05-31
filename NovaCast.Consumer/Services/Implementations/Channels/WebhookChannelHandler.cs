using System.Text.Json;
using NovaCast.Consumer.Models.Entities;
using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Implementations.Channels;

public class WebhookChannelHandler : IChannelHandler
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<WebhookChannelHandler> _logger;

    public WebhookChannelHandler(IHttpClientFactory httpClientFactory, ILogger<WebhookChannelHandler> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<DeliveryResult> DeliverAsync(
        NotificationEvent notification,
        TenantChannel channel,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var credentials = JsonSerializer.Deserialize<Dictionary<string, string>>(channel.Credentials);
            if (credentials is null || !credentials.TryGetValue("url", out var url))
                return DeliveryResult.Failed("Webhook URL not configured");

            var client = _httpClientFactory.CreateClient();
            var payload = JsonSerializer.Serialize(new
            {
                notification.Id,
                notification.Type,
                notification.Recipient,
                notification.Payload,
                notification.CreatedAt
            });

            var content = new StringContent(payload, System.Text.Encoding.UTF8, "application/json");

            if (credentials.TryGetValue("secret", out var secret))
                client.DefaultRequestHeaders.Add("X-NovaCast-Secret", secret);

            var response = await client.PostAsync(url, content, cancellationToken);

            _logger.LogInformation(
                "Webhook delivered for notification {NotificationId} — status {StatusCode}",
                notification.Id,
                response.StatusCode);

            return response.IsSuccessStatusCode
                ? DeliveryResult.Succeeded("webhook", null, ((int)response.StatusCode).ToString())
                : DeliveryResult.Failed($"Webhook returned {(int)response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook delivery failed for notification {NotificationId}", notification.Id);
            return DeliveryResult.Failed(ex.Message);
        }
    }
}