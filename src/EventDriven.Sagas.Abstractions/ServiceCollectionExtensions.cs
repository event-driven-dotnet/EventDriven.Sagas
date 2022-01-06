﻿using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;

namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Helper methods for adding sagas to dependency injection.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Register a concrete saga using application configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <typeparam name="TSagaWitConfig">Concrete saga type.</typeparam>
    /// <typeparam name="TSagaEntity">Saga entity.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Concrete implementation of <see cref="ISagaConfigSettings"/></typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSagaWitConfig,
        TSagaEntity, TSagaCommandDispatcher, TSagaConfigRepository,
        TCommandResultEvaluator,
        TSagaConfigSettings>(
        this IServiceCollection services, 
        IConfiguration configuration)
        where TSagaWitConfig : SagaWithConfig<TSagaEntity>, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
        where TSagaConfigSettings : ISagaConfigSettings, new()
    {
        var settings = new TSagaConfigSettings();
        configuration.GetSection(typeof(TSagaConfigSettings).Name).Bind(settings);
        return services.AddSaga<TSagaWitConfig, TSagaEntity,
            TSagaCommandDispatcher, TSagaConfigRepository,
            TCommandResultEvaluator>(settings.SagaConfigId);
    }

    /// <summary>
    /// Register a concrete saga using a saga configuration identifier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="sagaConfigId">Optional saga configuration identifier.</param>
    /// <typeparam name="TSagaWithConfig">Concrete saga type.</typeparam>
    /// <typeparam name="TSagaEntity">Saga entity.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSagaWithConfig,
        TSagaEntity, TSagaCommandDispatcher, TSagaConfigRepository,
        TCommandResultEvaluator>(
        this IServiceCollection services, 
        Guid? sagaConfigId = null)
        where TSagaWithConfig : SagaWithConfig<TSagaEntity>, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
        => services.AddSaga<TSagaWithConfig, TSagaEntity,
            TSagaCommandDispatcher, TSagaConfigRepository,
            TCommandResultEvaluator>(options =>
        {
            options.SagaConfigId = sagaConfigId;
        });

    /// <summary>
    /// Register a concrete saga using a configure method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">Method for configuring saga options.</param>
    /// <typeparam name="TSagaWithConfig">Concrete saga type.</typeparam>
    /// <typeparam name="TSagaEntity">Saga entity.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TSagaWithConfig,
        TSagaEntity, TSagaCommandDispatcher, TSagaConfigRepository,
        TCommandResultEvaluator>(
        this IServiceCollection services, 
        Action<SagaConfigurationOptions> configure)
        where TSagaWithConfig : SagaWithConfig<TSagaEntity>, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
    {
        var sagaConfigOptions = new SagaConfigurationOptions();
        configure(sagaConfigOptions);
        services.RegisterSagaTypes()
            .AddSingleton(sagaConfigOptions)
            .AddSingleton(sp =>
        {
            var commandDispatcher = sp.GetRequiredService<TSagaCommandDispatcher>();
            var configRepository = sp.GetRequiredService<TSagaConfigRepository>();
            var resultEvaluator = sp.GetRequiredService<TCommandResultEvaluator>();
            return new TSagaWithConfig
            {
                SagaCommandDispatcher = commandDispatcher,
                SagaConfigRepository = configRepository,
                CommandResultEvaluator = resultEvaluator
            };
        });
        return services;
    }

    /// <summary>
    /// Register command handlers.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
        services.RegisterHandlerTypes();

    private static IServiceCollection RegisterHandlerTypes(this IServiceCollection services) =>
        services.Scan(scan =>
        {
            scan.FromEntryAssembly()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandHandler<,>)))
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime();
        });

    private static IServiceCollection RegisterSagaTypes(this IServiceCollection services) =>
        services.Scan(scan =>
        {
            scan.FromEntryAssembly()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandDispatcher>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ISagaConfigRepository>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ICommandResultEvaluator>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandResultProcessor<>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithSingletonLifetime();
        });
}