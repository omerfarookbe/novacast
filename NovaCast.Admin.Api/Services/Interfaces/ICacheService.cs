namespace NovaCast.Admin.Api.Services.Interfaces;

public interface ICacheService
{
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
}