using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Configuration;
using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.Abstractions.Entities;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract record ConfigurableSaga : Saga
{
    /// <summary>
    /// Constructor.
    /// Note: To use <see cref="IServiceCollection"/>.AddSaga, inheritors must have a parameterless constructor. 
    /// </summary>
    // ReSharper disable once EmptyConstructor
    protected ConfigurableSaga()
    {
    }
    
    /// <summary>
    /// Saga configuration identifier.
    /// </summary>
    public Guid? SagaConfigId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Saga configuration name.
    /// </summary>
    public string? SagaConfigName { get; set; }

    /// <summary>
    /// Saga configuration options.
    /// </summary>
    public SagaConfigurationOptions? SagaConfigOptions { get; set; }

    /// <summary>
    /// Saga configuration repository.
    /// </summary>
    public ISagaConfigRepository? SagaConfigRepository { get; set; }

    /// <summary>
    /// Configure saga.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task ConfigureAsync()
    {
        if (SagaConfigOptions?.SagaConfigId != null && SagaConfigRepository != null)
        {
            var sagaConfig = await SagaConfigRepository
                .GetSagaConfigurationAsync(SagaConfigOptions.SagaConfigId.GetValueOrDefault());
            if (sagaConfig == null)
                throw new Exception($"Saga configuration with id '{SagaConfigOptions.SagaConfigId}' not present in Saga Configuration Repository.");
            SagaConfigId = sagaConfig.Id;
            SagaConfigName = sagaConfig.Name;
            Steps = sagaConfig.Steps;
        }
    }
    
    /// <inheritdoc />
    public override async Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        // Set steps from config
        await ConfigureAsync();
        
        // Start saga
        await base.StartSagaAsync(cancellationToken);
    }
}

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public abstract record ConfigurableSaga<TEntity> : ConfigurableSaga
    where TEntity : IEntity
{
    /// <inheritdoc />
    // ReSharper disable once EmptyConstructor
    protected ConfigurableSaga()
    {
    }

    /// <summary>
    /// Entity.
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public TEntity Entity { get; set; } = default!;

    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <param name="entity">Saga entity.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartSagaAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        Entity = entity;
        EntityId = entity.Id;
        await StartSagaAsync(cancellationToken);
    }
}