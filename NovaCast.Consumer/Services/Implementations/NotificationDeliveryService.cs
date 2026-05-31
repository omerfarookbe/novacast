using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NovaCast.Consumer.Data.Repositories.Interfaces;
using NovaCast.Consumer.Models.Entities;
using NovaCast.Consumer.Services.Interfaces;
using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer.Services.Implementations;

public class NotificationDeliveryService : INotificationDeliveryService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly ICacheService _cacheService;
    private readonly IChannelHandlerFactory _channelHandlerFactory;
    private readonly ILogger<NotificationDeliveryService> _logger;
    private const int MaxRetries = 3;

    private static string ChannelCacheKey(Guid tenantId, string channel) =>
        $"novacast:tenant:{tenantId}:channel:{channel}";

    public NotificationDeliveryService(
        INotificationRepository notificationRepository,
        ICacheService cacheService,
        IChannelHandlerFactory channelHandlerFactory,
        ILogger<NotificationDeliveryService> logger)
    {
        _notificationRepository = notificationRepository;
        _cacheService = cacheService;
        _channelHandlerFactory = channelHandlerFactory;
        _logger = logger;
    }

    public async Task ProcessAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Processing notification {NotificationId} for tenant {TenantId}",
            notificationEvent.Id,
            notificationEvent.TenantId);

        // Idempotency check — already delivered?
        var existing = await _notificationRepository.GetByIdAsync(notificationEvent.Id, cancellationToken);
        if (existing?.Status == "Delivered")
        {
            _logger.LogWarning(
                "Notification {NotificationId} already delivered — skipping",
                notificationEvent.Id);
            return;
        }

        // Get channel config from cache
        var cacheKey = ChannelCacheKey(notificationEvent.TenantId, notificationEvent.Channel.ToString());
        var tenantChannel = await _cacheService.GetAsync<TenantChannel>(cacheKey, cancellationToken);

        if (tenantChannel is null)
        {
            _logger.LogError(
                "Channel config not found for tenant {TenantId} channel {Channel}",
                notificationEvent.TenantId,
                notificationEvent.Channel);

            await _notificationRepository.UpdateDeadLetteredAsync(
                notificationEvent.Id,
                "Channel configuration not found",
                cancellationToken);
            return;
        }

        // Get the right handler for this channel
        var handler = _channelHandlerFactory.GetHandler(notificationEvent.Channel);

        // Attempt delivery with retries
        var retryCount = existing?.RetryCount ?? 0;
        DeliveryResult? result = null;

        while (retryCount <= MaxRetries)
        {
            result = await handler.DeliverAsync(notificationEvent, tenantChannel, cancellationToken);

            if (result.Success)
            {
                await _notificationRepository.UpdateDeliveredAsync(
                    notificationEvent.Id, result, cancellationToken);

                _logger.LogInformation(
                    "Notification {NotificationId} delivered successfully via {Provider}",
                    notificationEvent.Id,
                    result.Provider);
                return;
            }

            retryCount++;

            if (retryCount > MaxRetries)
                break;

            // Update status to retrying
            await _notificationRepository.UpdateRetryingAsync(
                notificationEvent.Id, retryCount, cancellationToken);

            // Exponential backoff — 2s, 4s, 8s
            var delay = TimeSpan.FromSeconds(Math.Pow(2, retryCount));
            _logger.LogWarning(
                "Notification {NotificationId} delivery failed — retry {RetryCount}/{MaxRetries} in {Delay}s",
                notificationEvent.Id,
                retryCount,
                MaxRetries,
                delay.TotalSeconds);

            await Task.Delay(delay, cancellationToken);
        }

        // All retries exhausted
        _logger.LogError(
            "Notification {NotificationId} permanently failed after {MaxRetries} retries",
            notificationEvent.Id,
            MaxRetries);

        await _notificationRepository.UpdateDeadLetteredAsync(
            notificationEvent.Id,
            result?.FailureReason ?? "Unknown error",
            cancellationToken);
    }
}