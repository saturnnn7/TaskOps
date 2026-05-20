using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TaskOps.Infrastructure.Persistence;

/// <summary>
/// Applies pending EF Core migrations automatically on application startup.
/// Runs before the HTTP pipeline starts — ensures DB is always up to date.
/// </summary>
public static class DatabaseMigrator
{
    public static async Task MigrateAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var logger = scope.ServiceProvider
            .GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var context = scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

            var pendingMigrations = await context.Database
                .GetPendingMigrationsAsync();

            var migrations = pendingMigrations.ToList();

            if (migrations.Count == 0)
            {
                logger.LogInformation("Database is up to date. No pending migrations.");
                return;
            }

            logger.LogInformation(
                "Applying {Count} pending migration(s): {Migrations}",
                migrations.Count,
                string.Join(", ", migrations));

            await context.Database.MigrateAsync();

            logger.LogInformation("Database migrations applied successfully.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while applying database migrations.");
            throw;
        }
    }
}