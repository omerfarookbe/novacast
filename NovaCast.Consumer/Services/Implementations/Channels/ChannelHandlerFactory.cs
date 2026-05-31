using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Enums;

namespace NovaCast.Consumer.Services.Implementations.Channels;

public class ChannelHandlerFactory : IChannelHandlerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ChannelHandlerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IChannelHandler GetHandler(NotificationChannel channel)
    {
        return channel switch
        {
            NotificationChannel.Email => _serviceProvider.GetRequiredService<EmailChannelHandler>(),
            NotificationChannel.Sms => _serviceProvider.GetRequiredService<SmsChannelHandler>(),
            NotificationChannel.Push => _serviceProvider.GetRequiredService<PushChannelHandler>(),
            NotificationChannel.Webhook => _serviceProvider.GetRequiredService<WebhookChannelHandler>(),
            _ => throw new NotSupportedException($"Channel {channel} is not supported")
        };
    }
}