using System.Reflection;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Configuration;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Mapping;
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
    /// Register a concrete saga using application configuration.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configuration">The application's <see cref="IConfiguration"/>.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaEntity">Saga entity.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Concrete implementation of <see cref="ISagaConfigSettings"/></typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TPersistableSaga,
        TSagaEntity, TSagaCommandDispatcher, TCommandResultEvaluator,
        TSagaConfigRepository, TSagaSnapshotRepository, TSagaConfigSettings>(
        this IServiceCollection services, 
        IConfiguration configuration)
        where TSagaEntity : IEntity
        where TPersistableSaga : PersistableSaga<TSagaEntity>, ICommandResultProcessor<TSagaEntity>, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TCommandResultEvaluator : ICommandResultEvaluator
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaConfigSettings : ISagaConfigSettings, new()
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
    {
        var settings = new TSagaConfigSettings();
        var configTypeName = typeof(TSagaConfigSettings).Name;
        var configSection = configuration.GetSection(configTypeName);
        configSection.Bind(settings);
        if (settings.SagaConfigId == Guid.Empty)
            throw new Exception($"'SagaConfigId' property not present in configuration section {configTypeName}");
        return services.AddSaga<TPersistableSaga, TSagaEntity, TSagaCommandDispatcher, TCommandResultEvaluator,
            TSagaConfigRepository, TSagaSnapshotRepository>(settings.SagaConfigId);
    }

    /// <summary>
    /// Register a concrete saga using a saga configuration identifier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="sagaConfigId">Optional saga configuration identifier.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaEntity">Saga entity.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TPersistableSaga,
        TSagaEntity, TSagaCommandDispatcher, TCommandResultEvaluator,
        TSagaConfigRepository, TSagaSnapshotRepository>(
        this IServiceCollection services, 
        Guid? sagaConfigId = null)
        where TSagaEntity : IEntity
        where TPersistableSaga : PersistableSaga<TSagaEntity>, ICommandResultProcessor<TSagaEntity>, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TCommandResultEvaluator : ICommandResultEvaluator
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
        => services.AddSaga<TPersistableSaga, TSagaEntity,
            TSagaCommandDispatcher, TCommandResultEvaluator,
            TSagaConfigRepository, TSagaSnapshotRepository>(options => 
            options.SagaConfigId = sagaConfigId);

    /// <summary>
    /// Register a concrete saga using a configure method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">Method for configuring saga options.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaEntity">Saga entity.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TCommandResultEvaluator">Command result evaluator type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<
        TPersistableSaga, TSagaEntity, TSagaCommandDispatcher, TCommandResultEvaluator,
        TSagaConfigRepository, TSagaSnapshotRepository>(
        this IServiceCollection services, 
        Action<SagaConfigurationOptions> configure)
        where TSagaEntity : IEntity
        where TPersistableSaga : PersistableSaga<TSagaEntity>, ICommandResultProcessor<TSagaEntity>, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TCommandResultEvaluator : ICommandResultEvaluator
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
    {
        var sagaConfigOptions = new SagaConfigurationOptions();
        configure(sagaConfigOptions);
        var resolver = new SagaCommandTypeResolver(Assembly.GetEntryAssembly()?.FullName);
        services.RegisterSagaTypes()
            .AddSingleton(sagaConfigOptions)
            .AddTransient<ISagaConfigRepository, TSagaConfigRepository>()
            .AddTransient<ISagaSnapshotRepository, TSagaSnapshotRepository>()
            .AddTransient<ISagaCommandTypeResolver, SagaCommandTypeResolver>(_ => resolver)
            .AddAutoMapper(cfg =>
            {
                SagaAutoMapperProfile.SagaCommandTypeResolver = resolver;
                cfg.AddProfile(new SagaAutoMapperProfile());
            }, typeof(SagaAutoMapperProfile))
            // .AddTransient<ICommandResultProcessor<TSagaEntity>>(sp =>
            // {
            //     return sp.GetRequiredService<TPersistableSaga>();
            //     // var commandDispatcher = sp.GetRequiredService<TSagaCommandDispatcher>();
            //     // var resultEvaluator = sp.GetRequiredService<TCommandResultEvaluator>();
            //     // var configRepository = sp.GetRequiredService<ISagaConfigRepository>();
            //     // var snapshotRepository = sp.GetRequiredService<ISagaSnapshotRepository>();
            //     // return new TPersistableSaga
            //     // {
            //     //     SagaCommandDispatcher = commandDispatcher,
            //     //     CommandResultEvaluator = resultEvaluator,
            //     //     SagaConfigRepository = configRepository,
            //     //     SagaSnapshotRepository = snapshotRepository,
            //     //     SagaConfigOptions = sagaConfigOptions
            //     // };
            // })
            .AddTransient(sp =>
            {
                var commandDispatcher = sp.GetRequiredService<TSagaCommandDispatcher>();
                var resultEvaluator = sp.GetRequiredService<TCommandResultEvaluator>();
                var configRepository = sp.GetRequiredService<ISagaConfigRepository>();
                var snapshotRepository = sp.GetRequiredService<ISagaSnapshotRepository>();
                return new TPersistableSaga
                {
                    SagaCommandDispatcher = commandDispatcher,
                    CommandResultEvaluator = resultEvaluator,
                    SagaConfigRepository = configRepository,
                    SagaSnapshotRepository = snapshotRepository,
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
                    .WithTransientLifetime();
        });

    private static IServiceCollection RegisterSagaTypes(this IServiceCollection services) =>
        services.Scan(scan =>
        {
            scan.FromEntryAssembly()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandDispatcher>())
                    // .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo<ICommandResultEvaluator>())
                    // .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ICommandResultProcessor<>)))
                    // .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsImplementedInterfaces()
                    .WithTransientLifetime();
        });
}