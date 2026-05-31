using Microsoft.EntityFrameworkCore;
using NovaCast.Consumer.Data;
using NovaCast.Consumer.Data.Repositories.Implementations;
using NovaCast.Consumer.Data.Repositories.Interfaces;
using NovaCast.Consumer.Services.Implementations;
using NovaCast.Consumer.Services.Implementations.Channels;
using NovaCast.Consumer.Services.Interfaces;
using NovaCast.Consumer.Services.Interfaces.Channels;
using StackExchange.Redis;

namespace NovaCast.Consumer.Extensions;

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
        services.AddScoped<INotificationRepository, NotificationRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<INotificationDeliveryService, NotificationDeliveryService>();
        services.AddScoped<EmailChannelHandler>();
        services.AddScoped<SmsChannelHandler>();
        services.AddScoped<PushChannelHandler>();
        services.AddScoped<WebhookChannelHandler>();
        services.AddScoped<IChannelHandlerFactory, ChannelHandlerFactory>();
        services.AddHttpClient();
        return services;
    }
}