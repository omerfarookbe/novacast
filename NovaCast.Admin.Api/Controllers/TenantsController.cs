using Microsoft.AspNetCore.Mvc;
using NovaCast.Admin.Api.Models.DTOs.Requests;
using NovaCast.Admin.Api.Models.DTOs.Responses;
using NovaCast.Admin.Api.Services.Interfaces;

namespace NovaCast.Admin.Api.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(ITenantService tenantService, ILogger<TenantsController> logger)
    {
        _tenantService = tenantService;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<TenantResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _tenantService.GetAllAsync(cancellationToken);
        return Ok(ApiResponse<List<TenantResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _tenantService.GetByIdAsync(id, cancellationToken);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Tenant not found"));

        return Ok(ApiResponse<TenantResponse>.Ok(result));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return BadRequest(ApiResponse<object>.Fail("Tenant name is required"));

        var result = await _tenantService.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id },
            ApiResponse<TenantResponse>.Ok(result, "Tenant created successfully"));
    }

    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(ApiResponse<TenantResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateTenantStatusRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _tenantService.UpdateStatusAsync(id, request, cancellationToken);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Tenant not found"));

        return Ok(ApiResponse<TenantResponse>.Ok(result, "Tenant status updated"));
    }

    [HttpPost("{id:guid}/channels")]
    [ProducesResponseType(typeof(ApiResponse<TenantChannelResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AssignChannel(
        Guid id,
        [FromBody] AssignChannelRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _tenantService.AssignChannelAsync(id, request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id },
            ApiResponse<TenantChannelResponse>.Ok(result, "Channel assigned successfully"));
    }

    [HttpDelete("{id:guid}/channels/{channel}")]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveChannel(
        Guid id,
        string channel,
        CancellationToken cancellationToken)
    {
        await _tenantService.RemoveChannelAsync(id, channel, cancellationToken);
        return Ok(ApiResponse<object>.Ok(new { }, "Channel removed successfully"));
    }
}