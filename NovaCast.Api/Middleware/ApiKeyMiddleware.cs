using System.Text.Json;
using NovaCast.Api.Common.Exceptions;
using NovaCast.Api.Models.DTOs.Responses;
using NovaCast.Api.Models.Entities;
using NovaCast.Api.Services.Interfaces;
using NovaCast.Contracts.Enums;

namespace NovaCast.Api.Middleware;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ApiKeyMiddleware> _logger;
    private const string ApiKeyHeader = "X-Api-Key";
    private const int RateLimitPerSecond = 10;

    public ApiKeyMiddleware(RequestDelegate next, ILogger<ApiKeyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService, ICacheService cacheService)
    {
        // Extract API key from header
        if (!context.Request.Headers.TryGetValue(ApiKeyHeader, out var apiKey) ||
            string.IsNullOrWhiteSpace(apiKey))
        {
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "API key is missing");
            return;
        }

        // Validate tenant
        var tenant = await tenantService.GetTenantByApiKeyAsync(apiKey!, context.RequestAborted);
        if (tenant is null)
        {
            _logger.LogWarning("Invalid API key attempt");
            await WriteErrorResponse(context, StatusCodes.Status401Unauthorized, "Invalid API key");
            return;
        }

        // Check tenant status
        if (tenant.Status != TenantStatus.Active)
        {
            _logger.LogWarning("Inactive tenant {TenantId} attempted access", tenant.Id);
            await WriteErrorResponse(context, StatusCodes.Status403Forbidden, "Tenant account is inactive");
            return;
        }

        // Check rate limit
        var rateLimitKey = $"novacast:ratelimit:{tenant.Id}";
        var currentCount = await cacheService.GetAsync<int>(rateLimitKey, context.RequestAborted);

        if (currentCount >= RateLimitPerSecond)
        {
            _logger.LogWarning("Rate limit exceeded for tenant {TenantId}", tenant.Id);
            context.Response.Headers.Append("Retry-After", "1");
            await WriteErrorResponse(context, StatusCodes.Status429TooManyRequests, "Rate limit exceeded. Maximum 10 requests per second");
            return;
        }

        // Increment rate limit counter
        await cacheService.SetAsync(rateLimitKey, currentCount + 1, TimeSpan.FromSeconds(1), context.RequestAborted);

        // Store tenant in context for controllers to use
        context.Items["Tenant"] = tenant;

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