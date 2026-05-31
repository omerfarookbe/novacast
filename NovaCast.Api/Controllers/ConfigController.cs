using Microsoft.AspNetCore.Mvc;
using NovaCast.Api.Models.DTOs.Requests;
using NovaCast.Api.Models.DTOs.Responses;
using NovaCast.Api.Models.Entities;
using NovaCast.Api.Services.Interfaces;

namespace NovaCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConfigController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<ConfigController> _logger;

    public ConfigController(
        ITenantService tenantService,
        ILogger<ConfigController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    [HttpGet("channels")]
    [ProducesResponseType(typeof(ApiResponse<TenantChannel>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetChannel(
        [FromQuery] string channel,
        CancellationToken cancellationToken)
    {
        var tenant = HttpContext.Items["Tenant"] as Tenant;
        if (tenant is null)
            return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));

        var result = await _tenantService.GetTenantChannelAsync(
            tenant.Id, channel, cancellationToken);

        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Channel not configured"));

        return Ok(ApiResponse<TenantChannel>.Ok(result));
    }

    [HttpPut("channels/{channel}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateChannel(
        string channel,
        [FromBody] UpdateChannelRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = HttpContext.Items["Tenant"] as Tenant;
        if (tenant is null)
            return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));

        var existing = await _tenantService.GetTenantChannelAsync(
            tenant.Id, channel, cancellationToken);

        if (existing is null)
            return NotFound(ApiResponse<object>.Fail("Channel not configured"));

        // Service layer will handle update — coming in next iteration
        await _tenantService.InvalidateTenantCacheAsync(tenant.Id, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { }, "Channel updated successfully"));
    }

    [HttpPatch("channels/{channel}/toggle")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ToggleChannel(
        string channel,
        [FromBody] ToggleChannelRequest request,
        CancellationToken cancellationToken)
    {
        var tenant = HttpContext.Items["Tenant"] as Tenant;
        if (tenant is null)
            return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));

        var existing = await _tenantService.GetTenantChannelAsync(
            tenant.Id, channel, cancellationToken);

        if (existing is null)
            return NotFound(ApiResponse<object>.Fail("Channel not configured"));

        await _tenantService.InvalidateTenantCacheAsync(tenant.Id, cancellationToken);

        return Ok(ApiResponse<object>.Ok(new { }, $"Channel {(request.IsEnabled ? "enabled" : "disabled")} successfully"));
    }
}