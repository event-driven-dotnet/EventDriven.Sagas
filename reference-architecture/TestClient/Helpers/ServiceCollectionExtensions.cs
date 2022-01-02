using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace TestClient.Helpers;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigSettings<TSettings>(this IServiceCollection services, IConfiguration config)
        where TSettings : class
    {
        services.Configure<TSettings>(
            config.GetSection(typeof(TSettings).Name));
        services.AddTransient(sp =>
            sp.GetRequiredService<IOptions<TSettings>>().Value);
        return services;
    }
}