using System.Text.Json;
using NovaCast.Api.Common.Exceptions;
using NovaCast.Api.Data.Repositories.Interfaces;
using NovaCast.Api.Models.DTOs.Requests;
using NovaCast.Api.Models.DTOs.Responses;
using NovaCast.Api.Models.Entities;
using NovaCast.Api.Services.Interfaces;
using NovaCast.Contracts.Enums;
using NovaCast.Contracts.Events;

namespace NovaCast.Api.Services.Implementations;

public class NotificationService : INotificationService
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IRequestLogRepository _requestLogRepository;
    private readonly ITenantService _tenantService;
    private readonly IKafkaProducerService _kafkaProducerService;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        INotificationRepository notificationRepository,
        IRequestLogRepository requestLogRepository,
        ITenantService tenantService,
        IKafkaProducerService kafkaProducerService,
        ILogger<NotificationService> logger)
    {
        _notificationRepository = notificationRepository;
        _requestLogRepository = requestLogRepository;
        _tenantService = tenantService;
        _kafkaProducerService = kafkaProducerService;
        _logger = logger;
    }

    public async Task<SendNotificationResponse> SendAsync(
        SendNotificationRequest request,
        Tenant tenant,
        CancellationToken cancellationToken = default)
    {
        // Validate notification type config
        var notificationType = await _tenantService.GetTenantNotificationTypeAsync(
            tenant.Id, request.Type, cancellationToken);

        if (notificationType is null)
            throw new ChannelNotConfiguredException(
                $"Notification type '{request.Type}' is not configured for your account");

        if (!notificationType.IsEnabled)
            throw new ChannelNotConfiguredException(
                $"Notification type '{request.Type}' is currently disabled");

        // Validate channel config
        var channel = await _tenantService.GetTenantChannelAsync(
            tenant.Id, notificationType.Channel.ToString(), cancellationToken);

        if (channel is null)
            throw new ChannelNotConfiguredException(
                $"Channel '{notificationType.Channel}' is not configured for your account");

        if (!channel.IsEnabled)
            throw new ChannelNotConfiguredException(
                $"Channel '{notificationType.Channel}' is currently disabled");

        // Log the request as accepted
        await _requestLogRepository.CreateAsync(new RequestLog
        {
            ApiKey = tenant.ApiKey,
            TenantId = tenant.Id,
            NotificationType = request.Type,
            Channel = notificationType.Channel,
            Status = "accepted",
            Payload = JsonSerializer.Serialize(request.Payload)
        }, cancellationToken);

        // Save notification with status received
        var notification = await _notificationRepository.CreateAsync(new Notification
        {
            TenantId = tenant.Id,
            Channel = notificationType.Channel,
            Type = request.Type,
            Recipient = request.Recipient,
            Payload = JsonSerializer.Serialize(request.Payload),
            Status = NotificationStatus.Received
        }, cancellationToken);

        // Publish to Kafka
        var notificationEvent = new NotificationEvent
        {
            Id = notification.Id,
            TenantId = tenant.Id,
            Type = request.Type,
            Channel = notificationType.Channel,
            Recipient = request.Recipient,
            Payload = request.Payload,
            CreatedAt = DateTime.UtcNow
        };

        var published = await _kafkaProducerService.PublishAsync(notificationEvent, cancellationToken);

        if (!published)
        {
            await _notificationRepository.UpdateStatusAsync(
                notification.Id,
                NotificationStatus.Failed,
                cancellationToken);

            _logger.LogError("Failed to publish notification {NotificationId} to Kafka", notification.Id);
            throw new Exception("Failed to queue notification. Please try again.");
        }

        await _notificationRepository.UpdatePublishedAsync(notification.Id, cancellationToken);

        _logger.LogInformation(
            "Notification {NotificationId} queued successfully for tenant {TenantId}",
            notification.Id,
            tenant.Id);

        return new SendNotificationResponse
        {
            NotificationId = notification.Id,
            Status = NotificationStatus.Published.ToString(),
            CreatedAt = notification.CreatedAt
        };
    }

    public async Task<NotificationResponse?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default)
    {
        var notification = await _notificationRepository.GetByIdAsync(id, cancellationToken);

        if (notification is null || notification.TenantId != tenantId)
            return null;

        return MapToResponse(notification);
    }

    public async Task<PagedResponse<NotificationResponse>> GetByTenantAsync(
        Guid tenantId,
        string? status,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        NotificationStatus? statusEnum = null;
        if (!string.IsNullOrEmpty(status) &&
            Enum.TryParse<NotificationStatus>(status, true, out var parsed))
            statusEnum = parsed;

        var notifications = await _notificationRepository.GetByTenantAsync(
            tenantId, statusEnum, page, pageSize, cancellationToken);

        var total = await _notificationRepository.CountByTenantAsync(
            tenantId, statusEnum, cancellationToken);

        return new PagedResponse<NotificationResponse>
        {
            Data = notifications.Select(MapToResponse),
            Page = page,
            PageSize = pageSize,
            Total = total
        };
    }

    private static NotificationResponse MapToResponse(Notification notification) => new()
    {
        Id = notification.Id,
        Type = notification.Type,
        Channel = notification.Channel.ToString(),
        Recipient = notification.Recipient,
        Status = notification.Status.ToString(),
        RetryCount = notification.RetryCount,
        Provider = notification.Provider,
        ProviderMessageId = notification.ProviderMessageId,
        FailureReason = notification.FailureReason,
        CreatedAt = notification.CreatedAt,
        PublishedAt = notification.PublishedAt,
        ProcessedAt = notification.ProcessedAt,
        LastRetriedAt = notification.LastRetriedAt
    };
}