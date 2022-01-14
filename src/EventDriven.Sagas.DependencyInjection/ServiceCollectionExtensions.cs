using System.Reflection;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Mapping;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Factories;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Scrutor;
using SagaConfigAutoMapperProfile = EventDriven.Sagas.Configuration.Abstractions.SagaConfigAutoMapperProfile;

namespace EventDriven.Sagas.DependencyInjection;

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
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Concrete implementation of <see cref="ISagaConfigSettings"/></typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TPersistableSaga, TSagaCommandDispatcher,
        TSagaConfigRepository, TSagaSnapshotRepository, TSagaConfigSettings>(
        this IServiceCollection services, 
        IConfiguration configuration)
        where TPersistableSaga : PersistableSaga, ISagaCommandResultHandler
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
        where TSagaConfigSettings : ISagaConfigSettings, new()
    {
        var settings = new TSagaConfigSettings();
        var configTypeName = typeof(TSagaConfigSettings).Name;
        var configSection = configuration.GetSection(configTypeName);
        configSection.Bind(settings);
        if (settings.SagaConfigId == Guid.Empty)
            throw new Exception($"'SagaConfigId' property not present in configuration section {configTypeName}");
        return services.AddSaga<TPersistableSaga, TSagaCommandDispatcher, TSagaConfigRepository, TSagaSnapshotRepository>(settings.SagaConfigId);
    }

    /// <summary>
    /// Register a concrete saga using a saga configuration identifier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="sagaConfigId">Optional saga configuration identifier.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TPersistableSaga,
        TSagaCommandDispatcher, TSagaConfigRepository, TSagaSnapshotRepository>(
        this IServiceCollection services, 
        Guid? sagaConfigId = null)
        where TPersistableSaga : PersistableSaga, ISagaCommandResultHandler
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
        => services.AddSaga<TPersistableSaga, TSagaCommandDispatcher, TSagaConfigRepository, TSagaSnapshotRepository>(options => 
            options.SagaConfigId = sagaConfigId);

    /// <summary>
    /// Register a concrete saga using a configure method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">Method for configuring saga options.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<
        TPersistableSaga, TSagaCommandDispatcher, TSagaConfigRepository, TSagaSnapshotRepository>(
        this IServiceCollection services, 
        Action<SagaConfigurationOptions> configure)
        where TPersistableSaga : PersistableSaga, ISagaCommandResultHandler
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
    {
        var sagaConfigOptions = new SagaConfigurationOptions();
        configure(sagaConfigOptions);
        var resolver = new SagaCommandTypeResolver(Assembly.GetEntryAssembly()?.FullName);
        services.RegisterSagaTypes()
            .AddSingleton(sagaConfigOptions)
            .AddSingleton<ISagaConfigRepository, TSagaConfigRepository>()
            .AddSingleton<ISagaSnapshotRepository, TSagaSnapshotRepository>()
            .AddSingleton<ISagaCommandTypeResolver, SagaCommandTypeResolver>(_ => resolver)
            .AddAutoMapper(cfg =>
            {
                SagaConfigAutoMapperProfile.SagaCommandTypeResolver = resolver;
                SagaPersistAutoMapperProfile.SagaCommandTypeResolver = resolver;
                cfg.AddProfile(new SagaConfigAutoMapperProfile());
                cfg.AddProfile(new SagaPersistAutoMapperProfile());
            }, typeof(SagaConfigAutoMapperProfile), typeof(SagaPersistAutoMapperProfile))
            .AddSingleton<ISagaFactory<TPersistableSaga>>(sp =>
            {
                var dispatcher = sp.GetRequiredService<TSagaCommandDispatcher>();
                var evaluators = sp.GetServices<ISagaCommandResultEvaluator>();
                var resultDispatchers = 
                    sp.GetServices<ISagaCommandResultDispatcher>();
                var configOptions = sp.GetRequiredService<SagaConfigurationOptions>();
                var configRepo = sp.GetRequiredService<ISagaConfigRepository>();
                var snapshotRepo = sp.GetRequiredService<ISagaSnapshotRepository>();
                return new PersistableSagaFactory<TPersistableSaga>(
                    dispatcher, evaluators, resultDispatchers, configOptions, configRepo, snapshotRepo);
            })
            .AddSingleton(sp =>
            {
                var factory = sp.GetRequiredService<ISagaFactory<TPersistableSaga>>();
                var saga = factory.CreateSaga();
                return saga;
            });
        return services;
    }

    /// <summary>
    /// Register command handlers implementing <see cref="ICommandHandler{TEntity,TCommand}"/>.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
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
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelf()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandResultEvaluator>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(ISagaCommandHandler<,>)))
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandResultDispatcher>())
                    .UsingRegistrationStrategy(RegistrationStrategy.Skip)
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime();
        });
}