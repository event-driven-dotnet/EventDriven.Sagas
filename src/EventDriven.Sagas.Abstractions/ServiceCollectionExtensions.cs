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
    /// <typeparam name="TSaga">Concrete saga type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSaga>(
        this IServiceCollection services, Guid? sagaConfigId = null)
        where TSaga : Saga
        => services.AddSaga<TSaga>(options =>
        {
            options.SagaConfigId = sagaConfigId;
        });

    /// <summary>
    /// Register a concrete saga using a configuration method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">Method for configuring saga options.</param>
    /// <typeparam name="TSaga">Concrete saga type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSaga>(
        this IServiceCollection services, 
        Action<SagaConfigurationOptions> configure)
        where TSaga : Saga
    {
        var sagaConfigOptions = new SagaConfigurationOptions();
        configure(sagaConfigOptions);
        services.AddSingleton(sagaConfigOptions);
        services.AddSingleton<TSaga>();
        return services;
    }
}