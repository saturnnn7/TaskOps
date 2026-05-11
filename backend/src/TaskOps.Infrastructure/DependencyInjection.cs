using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskOps.Application.Common.Interfaces;
using TaskOps.Domain.Interfaces;
using TaskOps.Infrastructure.Auth;
using TaskOps.Infrastructure.Persistence;

namespace TaskOps.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // PostgreSQL via EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                npgsql => npgsql.MigrationsAssembly(
                    typeof(AppDbContext).Assembly.FullName)));

        // Unit of Work — scoped per HTTP request
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auth services
        services.AddSingleton<TokenService>();
        services.AddSingleton<ITokenService>(sp => sp.GetRequiredService<TokenService>());
        services.AddScoped<IPasswordService, PasswordService>();

        return services;
    }
}