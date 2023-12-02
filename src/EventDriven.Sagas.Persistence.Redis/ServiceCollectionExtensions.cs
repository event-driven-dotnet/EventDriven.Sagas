using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace EventDriven.Sagas.Persistence.Redis;

/// <summary>
/// Helper methods for adding persistable saga Redis settings to dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register persistable saga Redis settings.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <param name="lifetime">Service lifetime.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSagaRedisSettings(this IServiceCollection services,
        IConfiguration configuration, ServiceLifetime lifetime = ServiceLifetime.Singleton)
    {
        const string name = "PersistableSagaRedisSettings:DistributedCacheEntryOptions";
        IConfigurationSection cacheOptionsSection = configuration.GetSection(name);
        if (!cacheOptionsSection.Exists())
            throw new Exception("Configuration section '" + name + "' not present in app settings.");

        services.Configure<DistributedCacheEntryOptions>(cacheOptionsSection);
        switch (lifetime)
        {
            case ServiceLifetime.Scoped:
                services.AddScoped(sp => sp.GetRequiredService<IOptions<DistributedCacheEntryOptions>>().Value);
                break;
            case ServiceLifetime.Transient:
                services.AddTransient(sp => sp.GetRequiredService<IOptions<DistributedCacheEntryOptions>>().Value);
                break;
            default:
                services.AddSingleton(sp => sp.GetRequiredService<IOptions<DistributedCacheEntryOptions>>().Value);
                break;
        }

        services.AddStackExchangeRedisCache(option =>
        {
            var redisSettingsSection = configuration.GetSection(nameof(PersistableSagaRedisSettings));
            var redisSettings = redisSettingsSection.Get<PersistableSagaRedisSettings>();
            option.Configuration = redisSettings.ConnectionString;
            option.InstanceName = redisSettings.InstanceName;
        });
        return services;
    }
}