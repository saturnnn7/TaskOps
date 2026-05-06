using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TaskOps.Domain.Common;

namespace TaskOps.Infrastructure.Persistence.Interceptors;

/// <summary>
/// Automatically sets CreatedAt and UpdatedAt timestamps on every save.
/// Runs before every INSERT and UPDATE operation — no manual date setting needed.
/// </summary>
public sealed class AuditInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        SetAuditTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetAuditTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void SetAuditTimestamps(DbContext? context)
    {
        if (context is null) return;

        var now = DateTime.UtcNow;

        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;
                
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }

        }
    }
}