using NovaCast.Admin.Api.Middleware;

namespace NovaCast.Admin.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseAdminApiKeyAuthentication(this WebApplication app)
    {
        app.UseMiddleware<AdminApiKeyMiddleware>();
        return app;
    }
}