using NovaCast.Consumer.Services.Interfaces.Channels;
using NovaCast.Contracts.Enums;

namespace NovaCast.Consumer.Services.Interfaces.Channels;

public interface IChannelHandlerFactory
{
    IChannelHandler GetHandler(NotificationChannel channel);
}