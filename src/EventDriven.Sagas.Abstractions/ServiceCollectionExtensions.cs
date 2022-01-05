using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Helper methods for adding sagas to dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register a concrete saga using a configuration method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <typeparam name="TSagaConfig">Concrete saga type.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Concrete implementation of <see cref="ISagaConfigSettings"/></typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSagaConfig,
        TSagaCommandDispatcher, TSagaConfigRepository,
        TCommandResultEvaluator,
        TSagaConfigSettings>(
        this IServiceCollection services, 
        IConfiguration configuration)
        where TSagaConfig : SagaConfig, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
        where TSagaConfigSettings : ISagaConfigSettings, new()
    {
        var settings = new TSagaConfigSettings();
        configuration.GetSection(typeof(TSagaConfigSettings).Name).Bind(settings);
        return services.AddSaga<TSagaConfig,
            TSagaCommandDispatcher, TSagaConfigRepository,
            TCommandResultEvaluator>(settings.SagaConfigId);
    }

    /// <summary>
    /// Register a concrete saga using an optional configuration identifier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="sagaConfigId">Optional saga configuration identifier.</param>
    /// <typeparam name="TSagaConfig">Concrete saga type.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSagaConfig,
        TSagaCommandDispatcher, TSagaConfigRepository,
        TCommandResultEvaluator>(
        this IServiceCollection services, 
        Guid? sagaConfigId = null)
        where TSagaConfig : SagaConfig, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
        => services.AddSaga<TSagaConfig,
            TSagaCommandDispatcher, TSagaConfigRepository,
            TCommandResultEvaluator>(options =>
        {
            options.SagaConfigId = sagaConfigId;
        });

    /// <summary>
    /// Register a concrete saga using a configuration method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">Method for configuring saga options.</param>
    /// <typeparam name="TSagaConfig">Concrete saga type.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSagaConfig,
        TSagaCommandDispatcher, TSagaConfigRepository,
        TCommandResultEvaluator>(
        this IServiceCollection services, 
        Action<SagaConfigurationOptions> configure)
        where TSagaConfig : SagaConfig, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
    {
        var sagaConfigOptions = new SagaConfigurationOptions();
        configure(sagaConfigOptions);
        services.AddSingleton(sagaConfigOptions);
        services.AddSingleton(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<TSagaCommandDispatcher>();
            var configRepository = sp.GetRequiredService<TSagaConfigRepository>();
            var resultEvaluator = sp.GetRequiredService<TCommandResultEvaluator>();
            return new TSagaConfig
            {
                SagaCommandDispatcher = commandDispatcher,
                SagaConfigRepository = configRepository,
                CommandResultEvaluator = resultEvaluator
            };
        });
        return services;
    }
}