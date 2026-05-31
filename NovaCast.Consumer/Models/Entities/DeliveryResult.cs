namespace NovaCast.Consumer.Models.Entities;

public class DeliveryResult
{
    public bool Success { get; set; }
    public string? Provider { get; set; }
    public string? ProviderMessageId { get; set; }
    public string? ProviderResponse { get; set; }
    public string? FailureReason { get; set; }

    public static DeliveryResult Succeeded(string provider, string? messageId, string? response) => new()
    {
        Success = true,
        Provider = provider,
        ProviderMessageId = messageId,
        ProviderResponse = response
    };

    public static DeliveryResult Failed(string reason) => new()
    {
        Success = false,
        FailureReason = reason
    };
}