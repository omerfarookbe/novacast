using Microsoft.EntityFrameworkCore;
using NovaCast.Admin.Api.Data;
using NovaCast.Admin.Api.Data.Repositories.Implementations;
using NovaCast.Admin.Api.Data.Repositories.Interfaces;
using NovaCast.Admin.Api.Services.Implementations;
using NovaCast.Admin.Api.Services.Interfaces;
using StackExchange.Redis;

namespace NovaCast.Admin.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));
        return services;
    }

    public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Redis")
            ?? throw new InvalidOperationException("Redis connection string is not configured");

        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(connectionString));

        services.AddScoped<ICacheService, CacheService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        return services;
    }
}