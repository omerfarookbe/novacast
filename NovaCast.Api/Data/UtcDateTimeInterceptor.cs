using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace NovaCast.Api.Data;

public class UtcDateTimeInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        ConvertToUtc(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        ConvertToUtc(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void ConvertToUtc(DbContext? context)
    {
        if (context is null) return;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            foreach (var property in entry.Properties)
            {
                if (property.CurrentValue is DateTime dateTime &&
                    dateTime.Kind != DateTimeKind.Utc)
                {
                    property.CurrentValue = DateTime.SpecifyKind(
                        dateTime, DateTimeKind.Utc);
                }
            }
        }
    }
}