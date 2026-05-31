using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using NovaCast.Api.Data;
using NovaCast.Api.Data.Repositories.Implementations;
using NovaCast.Api.Data.Repositories.Interfaces;
using NovaCast.Api.Services.Implementations;
using NovaCast.Api.Services.Interfaces;
using StackExchange.Redis;

namespace NovaCast.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<UtcDateTimeInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
                   .AddInterceptors(sp.GetRequiredService<UtcDateTimeInterceptor>());
        });

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

    public static IServiceCollection AddKafka(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IKafkaProducerService, KafkaProducerService>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IRequestLogRepository, RequestLogRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITenantService, TenantService>();
        services.AddScoped<INotificationService, NotificationService>();
        return services;
    }
}