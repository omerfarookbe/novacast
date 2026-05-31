using Microsoft.AspNetCore.Mvc;
using NovaCast.Api.Models.DTOs.Requests;
using NovaCast.Api.Models.DTOs.Responses;
using NovaCast.Api.Models.Entities;
using NovaCast.Api.Services.Interfaces;

namespace NovaCast.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(
        INotificationService notificationService,
        ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<SendNotificationResponse>), StatusCodes.Status202Accepted)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Send(
        [FromBody] SendNotificationRequest request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
            return BadRequest(ApiResponse<object>.Fail("Invalid request"));

        var tenant = HttpContext.Items["Tenant"] as Tenant;
        if (tenant is null)
            return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));

        var result = await _notificationService.SendAsync(request, tenant, cancellationToken);
        return Accepted(ApiResponse<SendNotificationResponse>.Ok(result, "Notification queued successfully"));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<NotificationResponse>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var tenant = HttpContext.Items["Tenant"] as Tenant;
        if (tenant is null)
            return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));

        pageSize = Math.Min(pageSize, 100);

        var result = await _notificationService.GetByTenantAsync(
            tenant.Id, status, page, pageSize, cancellationToken);

        return Ok(ApiResponse<PagedResponse<NotificationResponse>>.Ok(result));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<NotificationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var tenant = HttpContext.Items["Tenant"] as Tenant;
        if (tenant is null)
            return Unauthorized(ApiResponse<object>.Fail("Unauthorized"));

        var result = await _notificationService.GetByIdAsync(id, tenant.Id, cancellationToken);
        if (result is null)
            return NotFound(ApiResponse<object>.Fail("Notification not found"));

        return Ok(ApiResponse<NotificationResponse>.Ok(result));
    }
}