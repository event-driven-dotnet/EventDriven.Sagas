using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;

// ReSharper disable once CheckNamespace
namespace EventDriven.DependencyInjection.URF.Mongo;

/// <summary>
/// Helper methods for configuring services with dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register MongoDB collection with dependency injection.
    /// </summary>
    /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the service to.</param>
    /// <param name="config">The application's <see cref="T:Microsoft.Extensions.Configuration.IConfiguration" />.</param>
    /// <typeparam name="TAppSettings">Strongly typed app settings class.</typeparam>
    /// <typeparam name="TEntity">Entity type.</typeparam>
    /// <typeparam name="TDto">DTO type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddMongoDbSettings<TAppSettings, TEntity, TDto>(
        this IServiceCollection services,
        IConfiguration config)
        where TAppSettings : class, IMongoDbSettings
        where TEntity : class
        where TDto : class
    {
        services.AddAppSettings<TAppSettings>(config);
        services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<TAppSettings>();
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            return database.GetCollection<TEntity>(settings.CollectionName);
        });
        services.AddSingleton<IDocumentRepository<TDto>, DocumentRepository<TDto>>();
        return services;
    }
}
