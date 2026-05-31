using NovaCast.Api.Middleware;

namespace NovaCast.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseApiKeyAuthentication(this WebApplication app)
    {
        app.UseMiddleware<ApiKeyMiddleware>();
        return app;
    }
}