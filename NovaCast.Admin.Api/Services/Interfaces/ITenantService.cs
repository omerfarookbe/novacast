using NovaCast.Admin.Api.Models.DTOs.Requests;
using NovaCast.Admin.Api.Models.DTOs.Responses;

namespace NovaCast.Admin.Api.Services.Interfaces;

public interface ITenantService
{
    Task<List<TenantResponse>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<TenantResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<TenantResponse> CreateAsync(CreateTenantRequest request, CancellationToken cancellationToken = default);
    Task<TenantResponse?> UpdateStatusAsync(Guid id, UpdateTenantStatusRequest request, CancellationToken cancellationToken = default);
    Task<TenantChannelResponse> AssignChannelAsync(Guid tenantId, AssignChannelRequest request, CancellationToken cancellationToken = default);
    Task RemoveChannelAsync(Guid tenantId, string channel, CancellationToken cancellationToken = default);
}