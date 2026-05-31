using System.Text.Json;
using Confluent.Kafka;
using NovaCast.Api.Services.Interfaces;
using NovaCast.Contracts.Events;

namespace NovaCast.Api.Services.Implementations;

public class KafkaProducerService : IKafkaProducerService, IDisposable
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducerService> _logger;
    private const string Topic = "notifications";

    public KafkaProducerService(IConfiguration configuration, ILogger<KafkaProducerService> logger)
    {
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = configuration["Kafka:BootstrapServers"],
            Acks = Acks.All,
            EnableIdempotence = true,
            MessageSendMaxRetries = 3,
            RetryBackoffMs = 1000
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task<bool> PublishAsync(NotificationEvent notificationEvent, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new Message<string, string>
            {
                Key = notificationEvent.TenantId.ToString(),
                Value = JsonSerializer.Serialize(notificationEvent)
            };

            var result = await _producer.ProduceAsync(Topic, message, cancellationToken);

            _logger.LogInformation(
                "Published notification {NotificationId} to topic {Topic} partition {Partition} offset {Offset}",
                notificationEvent.Id,
                result.Topic,
                result.Partition,
                result.Offset);

            return true;
        }
        catch (ProduceException<string, string> ex)
        {
            _logger.LogError(ex,
                "Failed to publish notification {NotificationId} to Kafka — {Reason}",
                notificationEvent.Id,
                ex.Error.Reason);

            return false;
        }
    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(5));
        _producer.Dispose();
    }
}