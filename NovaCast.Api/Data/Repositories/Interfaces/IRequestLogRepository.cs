using NovaCast.Api.Models.Entities;

namespace NovaCast.Api.Data.Repositories.Interfaces;

public interface IRequestLogRepository
{
    Task CreateAsync(RequestLog requestLog, CancellationToken cancellationToken = default);
}