namespace NovaCast.Api.Common.Exceptions;

public class ChannelNotConfiguredException : Exception
{
    public ChannelNotConfiguredException(string message) : base(message)
    {
    }
}