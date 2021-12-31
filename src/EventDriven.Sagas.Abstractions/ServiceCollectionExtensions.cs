using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Helper methods for adding sagas to dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register a concrete saga using an optional configuration identifier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="sagaConfigId">Optional saga configuration identifier.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSaga>(
        this IServiceCollection services, Guid? sagaConfigId = null)
        where TSaga : Saga
    {
        services.AddSingleton(provider =>
        {
            var saga = provider.GetRequiredService<TSaga>();
            saga.SagaConfigId = sagaConfigId;
            var configRepo = provider.GetService<ISagaConfigRepository>();
            if (configRepo != null) saga.ConfigRepository = configRepo;
            return saga;
        });
        return services;
    }
}