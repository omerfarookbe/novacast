using System.Text.Json;
using Confluent.Kafka;
using NovaCast.Consumer.Services.Interfaces;
using NovaCast.Contracts.Events;

namespace NovaCast.Consumer;

public class KafkaConsumerWorker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<KafkaConsumerWorker> _logger;
    private const string Topic = "notifications";
    private const string DlqTopic = "notifications.dlq";
    private const string ConsumerGroup = "novacast";

    public KafkaConsumerWorker(
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration,
        ILogger<KafkaConsumerWorker> logger)
    {
        _scopeFactory = scopeFactory;
        _configuration = configuration;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Kafka consumer worker starting...");

        var config = new ConsumerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"],
            GroupId = ConsumerGroup,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,        // manual commit
            EnableAutoOffsetStore = false    // we control offset storage
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        consumer.Subscribe(Topic);

        _logger.LogInformation("Subscribed to topic {Topic}", Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(TimeSpan.FromMilliseconds(500));
                if (consumeResult is null) continue;

                _logger.LogInformation(
                    "Received message from topic {Topic} partition {Partition} offset {Offset}",
                    consumeResult.Topic,
                    consumeResult.Partition,
                    consumeResult.Offset);

                var notificationEvent = JsonSerializer.Deserialize<NotificationEvent>(consumeResult.Message.Value);
                if (notificationEvent is null)
                {
                    _logger.LogWarning("Failed to deserialize message — skipping");
                    consumer.StoreOffset(consumeResult);
                    consumer.Commit(consumeResult);
                    continue;
                }

                // Process in a new scope — scoped services like DbContext
                using var scope = _scopeFactory.CreateScope();
                var deliveryService = scope.ServiceProvider
                    .GetRequiredService<INotificationDeliveryService>();

                try
                {
                    await deliveryService.ProcessAsync(notificationEvent, stoppingToken);

                    // Commit offset only after successful processing
                    consumer.StoreOffset(consumeResult);
                    consumer.Commit(consumeResult);

                    _logger.LogInformation(
                        "Committed offset {Offset} for partition {Partition}",
                        consumeResult.Offset,
                        consumeResult.Partition);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        "Failed to process notification {NotificationId} — sending to DLQ",
                        notificationEvent.Id);

                    await PublishToDlqAsync(notificationEvent, ex.Message);

                    // Commit even on failure — DLQ handles it
                    consumer.StoreOffset(consumeResult);
                    consumer.Commit(consumeResult);
                }
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Consumer worker stopping...");
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in consumer loop");
                await Task.Delay(1000, stoppingToken);
            }
        }

        consumer.Close();
        _logger.LogInformation("Kafka consumer worker stopped");
    }

    private async Task PublishToDlqAsync(NotificationEvent notificationEvent, string reason)
    {
        var config = new ProducerConfig
        {
            BootstrapServers = _configuration["Kafka:BootstrapServers"]
        };

        using var producer = new ProducerBuilder<string, string>(config).Build();

        var message = new Message<string, string>
        {
            Key = notificationEvent.TenantId.ToString(),
            Value = JsonSerializer.Serialize(new
            {
                Event = notificationEvent,
                FailureReason = reason,
                FailedAt = DateTime.UtcNow
            })
        };

        await producer.ProduceAsync(DlqTopic, message);

        _logger.LogWarning(
            "Notification {NotificationId} published to DLQ — reason: {Reason}",
            notificationEvent.Id,
            reason);
    }
}