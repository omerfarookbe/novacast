namespace NovaCast.Api.Common.Exceptions;

public class RateLimitExceededException : Exception
{
    public RateLimitExceededException(string message) : base(message)
    {
    }
}