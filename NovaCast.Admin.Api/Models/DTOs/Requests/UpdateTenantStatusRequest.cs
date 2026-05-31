namespace NovaCast.Admin.Api.Models.DTOs.Requests;

public class UpdateTenantStatusRequest
{
    public string Status { get; set; } = default!; // active | inactive
}