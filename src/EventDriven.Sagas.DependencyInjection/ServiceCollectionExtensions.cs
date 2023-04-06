using System.Reflection;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Mapping;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Factories;
using EventDriven.Sagas.Persistence.Abstractions.Pools;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
    /// <param name="assemblyMarkerTypes">Assembly marker types.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Saga configuration settings type.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <typeparam name="TPersistableSagaRepository">Persistable saga repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TPersistableSaga,
        TSagaConfigSettings, TSagaCommandDispatcher,
        TSagaConfigRepository, TSagaSnapshotRepository, TPersistableSagaRepository>(
        this IServiceCollection services, 
        IConfiguration configuration,
        params Type[] assemblyMarkerTypes)
        where TPersistableSaga : PersistableSaga, ISagaCommandResultHandler
        where TSagaConfigSettings : class, ISagaConfigSettings, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
        where TPersistableSagaRepository : class, IPersistableSagaRepository<TPersistableSaga>
    {
        var settings = new TSagaConfigSettings();
        var configTypeName = typeof(TSagaConfigSettings).Name;
        var configSection = configuration.GetSection(configTypeName);
        configSection.Bind(settings);
        if (settings.SagaConfigId == Guid.Empty)
            throw new Exception($"'SagaConfigId' property not present in configuration section {configTypeName}");
        return services.AddSaga<TPersistableSaga, TSagaConfigSettings, TSagaCommandDispatcher,
            TSagaConfigRepository, TSagaSnapshotRepository, TPersistableSagaRepository>(
            settings.SagaConfigId, settings.OverrideLockCheck, assemblyMarkerTypes);
    }

    /// <summary>
    /// Register a concrete saga using a saga configuration identifier.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="sagaConfigId">Optional saga configuration identifier.</param>
    /// <param name="overrideLockCheck">True to override lock check.</param>
    /// <param name="assemblyMarkerTypes">Assembly marker types.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Saga configuration settings type.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <typeparam name="TPersistableSagaRepository">Persistable saga repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<TPersistableSaga,
        TSagaConfigSettings, TSagaCommandDispatcher,
        TSagaConfigRepository, TSagaSnapshotRepository, TPersistableSagaRepository>(
        this IServiceCollection services, 
        Guid? sagaConfigId = null,
        bool overrideLockCheck = false,
        params Type[] assemblyMarkerTypes)
        where TPersistableSaga : PersistableSaga, ISagaCommandResultHandler
        where TSagaConfigSettings : class, ISagaConfigSettings, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
        where TPersistableSagaRepository : class, IPersistableSagaRepository<TPersistableSaga>
        => services.AddSaga<TPersistableSaga, TSagaConfigSettings, TSagaCommandDispatcher,
            TSagaConfigRepository, TSagaSnapshotRepository, TPersistableSagaRepository>(options =>
        {
            options.SagaConfigId = sagaConfigId;
            options.OverrideLockCheck = overrideLockCheck;
        }, assemblyMarkerTypes);

    /// <summary>
    /// Register a concrete saga using a configure method.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the service to.</param>
    /// <param name="configure">Method for configuring saga options.</param>
    /// <param name="assemblyMarkerTypes">Assembly marker types.</param>
    /// <typeparam name="TPersistableSaga">Concrete saga type that extends PersistableSaga.</typeparam>
    /// <typeparam name="TSagaConfigSettings">Saga configuration settings type.</typeparam>
    /// <typeparam name="TSagaCommandDispatcher">Saga command dispatcher type.</typeparam>
    /// <typeparam name="TSagaConfigRepository">Saga config repository type.</typeparam>
    /// <typeparam name="TSagaSnapshotRepository">Saga snapshot repository type.</typeparam>
    /// <typeparam name="TPersistableSagaRepository">Persistable saga repository type.</typeparam>
    /// <returns>A reference to this instance after the operation has completed.</returns>
    public static IServiceCollection AddSaga<
        TPersistableSaga, TSagaConfigSettings, TSagaCommandDispatcher,
        TSagaConfigRepository, TSagaSnapshotRepository, TPersistableSagaRepository>(
        this IServiceCollection services, 
        Action<TSagaConfigSettings> configure,
        params Type[] assemblyMarkerTypes)
        where TPersistableSaga : PersistableSaga, ISagaCommandResultHandler
        where TSagaConfigSettings : class, ISagaConfigSettings, new()
        where TSagaCommandDispatcher : ISagaCommandDispatcher
        where TSagaConfigRepository : class, ISagaConfigRepository
        where TSagaSnapshotRepository : class, ISagaSnapshotRepository
        where TPersistableSagaRepository : class, IPersistableSagaRepository<TPersistableSaga>
    {
        var sagaConfigOptions = new TSagaConfigSettings();
        configure(sagaConfigOptions);
        var assemblyName = GetSagaCommandsAssemblyName(assemblyMarkerTypes);
        var resolver = new SagaCommandTypeResolver(assemblyName);
        services.RegisterSagaTypes(assemblyMarkerTypes)
            .AddSingleton(sagaConfigOptions)
            .AddSingleton<ISagaConfigRepository, TSagaConfigRepository>()
            .AddSingleton<ISagaSnapshotRepository, TSagaSnapshotRepository>()
            .AddSingleton<IPersistableSagaRepository<TPersistableSaga>, TPersistableSagaRepository>()
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
                var checkLockHandlers = sp.GetServices<ICheckSagaLockCommandHandler>();
                var configOptions = sp.GetRequiredService<TSagaConfigSettings>();
                var configRepo = sp.GetRequiredService<ISagaConfigRepository>();
                var snapshotRepo = sp.GetRequiredService<ISagaSnapshotRepository>();
                return new PersistableSagaFactory<TPersistableSaga>(
                    dispatcher, evaluators, checkLockHandlers,
                    configOptions, configRepo, snapshotRepo);
            })
            .AddSingleton<ISagaPool<TPersistableSaga>>(sp =>
            {
                var factory = sp.GetRequiredService<ISagaFactory<TPersistableSaga>>();
                var repository = sp.GetRequiredService<IPersistableSagaRepository<TPersistableSaga>>();
                var configOptions = sp.GetRequiredService<TSagaConfigSettings>();
                var resultDispatchers = 
                    sp.GetServices<ISagaCommandResultDispatcher>().DistinctBy(e => e.GetType()).ToList();
                var sagaPool = new PersistableSagaPool<TPersistableSaga>(factory, resultDispatchers, repository, configOptions.OverrideLockCheck);
                return sagaPool;
            });
        return services;
    }

    private static IServiceCollection RegisterSagaTypes(this IServiceCollection services,
        params Type[] assemblyMarkerTypes) =>
        services.Scan(scan =>
        {
            var typeSelector = assemblyMarkerTypes.Length > 0
                ? scan.FromAssembliesOf(assemblyMarkerTypes)
                : scan.FromEntryAssembly();
            typeSelector
                .AddClasses(classes => classes.AssignableTo<ISagaCommandDispatcher>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandResultEvaluator>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandHandler>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ICheckSagaLockCommandHandler>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<ISagaCommandResultDispatcher>())
                    .AsSelfWithInterfaces()
                    .WithSingletonLifetime();
        });

    private static string? GetSagaCommandsAssemblyName(params Type[] assemblyMarkerTypes) =>
        assemblyMarkerTypes.SelectMany(t => t.Assembly.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(ISagaCommand)))
            .Select(t => t.Assembly.FullName)
            .FirstOrDefault() ?? Assembly.GetEntryAssembly()?.FullName;
}