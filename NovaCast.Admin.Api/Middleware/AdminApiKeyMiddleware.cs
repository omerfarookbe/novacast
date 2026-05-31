using System.Text.Json;
using NovaCast.Admin.Api.Models.DTOs.Responses;

namespace NovaCast.Admin.Api.Middleware;

public class AdminApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AdminApiKeyMiddleware> _logger;
    private const string ApiKeyHeader = "X-Admin-Api-Key";

    public AdminApiKeyMiddleware(RequestDelegate next, ILogger<AdminApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
    {
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey) ||
            string.IsNullOrWhiteSpace(apiKey))
        {
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Admin API key is missing");
            return;
        }

        var validKey = configuration["AdminApiKey"];
        if (apiKey != validKey)
        {
            _logger.LogWarning("Invalid admin API key attempt");
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Invalid admin API key");
            return;
        }

        await _next(context);
    }

    private static async Task WriteErrorResponse(HttpContext context, int statusCode, string message)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.Fail(message);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}