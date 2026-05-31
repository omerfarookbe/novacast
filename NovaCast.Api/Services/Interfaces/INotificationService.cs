using NovaCast.Api.Models.DTOs.Requests;
using NovaCast.Api.Models.DTOs.Responses;
using NovaCast.Api.Models.Entities;

namespace NovaCast.Api.Services.Interfaces;

public interface INotificationService
{
    Task<SendNotificationResponse> SendAsync(SendNotificationRequest request, Tenant tenant, CancellationToken cancellationToken = default);
    Task<NotificationResponse?> GetByIdAsync(Guid id, Guid tenantId, CancellationToken cancellationToken = default);
    Task<PagedResponse<NotificationResponse>> GetByTenantAsync(Guid tenantId, string? status, int page, int pageSize, CancellationToken cancellationToken = default);
}