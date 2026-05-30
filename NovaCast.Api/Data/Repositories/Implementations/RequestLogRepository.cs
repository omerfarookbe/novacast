using NovaCast.Api.Data.Repositories.Interfaces;
using NovaCast.Api.Models.Entities;

namespace NovaCast.Api.Data.Repositories.Implementations;

public class RequestLogRepository : IRequestLogRepository
{
    private readonly AppDbContext _context;

    public RequestLogRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RequestLog requestLog, CancellationToken cancellationToken = default)
    {
        requestLog.CreatedAt = DateTime.UtcNow;
        await _context.RequestLogs.AddAsync(requestLog, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}