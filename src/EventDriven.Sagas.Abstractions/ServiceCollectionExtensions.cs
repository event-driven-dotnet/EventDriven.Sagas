using System.Reflection;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Configuration;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Mapping;
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
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
        where TSagaConfigSettings : ISagaConfigSettings, new()
    {
        var settings = new TSagaConfigSettings();
        var configTypeName = typeof(TSagaConfigSettings).Name;
        var configSection = configuration.GetSection(configTypeName);
        configSection.Bind(settings);
        if (settings.SagaConfigId == Guid.Empty)
            throw new Exception($"'SagaConfigId' property not present in configuration section {configTypeName}");
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
        where TSagaConfigRepository : class, ISagaConfigRepository
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
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TCommandResultEvaluator : ICommandResultEvaluator
    {
        var sagaConfigOptions = new SagaConfigurationOptions();
        configure(sagaConfigOptions);
        var resolver = new SagaCommandTypeResolver(Assembly.GetEntryAssembly()?.FullName);
        services.RegisterSagaTypes()
            .AddSingleton(sagaConfigOptions)
            .AddSingleton<TSagaConfigRepository>()
            .AddSingleton<ISagaCommandTypeResolver, SagaCommandTypeResolver>(_ => resolver)
            .AddAutoMapper(cfg =>
            {
                SagaAutoMapperProfile.SagaCommandTypeResolver = resolver;
                cfg.AddProfile(new SagaAutoMapperProfile());
            }, typeof(SagaAutoMapperProfile))
            .AddSingleton(sp =>
            {
                var commandDispatcher = sp.GetRequiredService<TSagaCommandDispatcher>();
                var configRepository = sp.GetRequiredService<TSagaConfigRepository>();
                var resultEvaluator = sp.GetRequiredService<TCommandResultEvaluator>();
                return new TSagaWithConfig
                {
                    SagaCommandDispatcher = commandDispatcher,
                    SagaConfigRepository = configRepository,
                    CommandResultEvaluator = resultEvaluator,
                    SagaConfigOptions = sagaConfigOptions
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
                // TODO: Replace with ISagaConfigRepository?
                // .AddClasses(classes => classes.AssignableTo<ISagaConfigDtoRepository>())
                //     .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                //     .AsSelf()
                //     .WithSingletonLifetime()
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