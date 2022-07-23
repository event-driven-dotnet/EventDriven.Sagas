using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;

namespace EventDriven.Sagas.Configuration.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class ConfigurableSaga : Saga
{
    /// <inheritdoc />
    protected ConfigurableSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) : 
        base(sagaCommandDispatcher, commandResultEvaluators)
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
                .GetAsync(SagaConfigOptions.SagaConfigId.GetValueOrDefault());
            if (sagaConfig == null)
                throw new Exception($"Saga configuration with id '{SagaConfigOptions.SagaConfigId}' not present in Saga Configuration Repository.");
            SagaConfigId = sagaConfig.Id;
            SagaConfigName = sagaConfig.Name;
            Steps = sagaConfig.Steps;
        }
    }
    
    /// <inheritdoc />
    public override async Task StartSagaAsync(Guid entityId = default, CancellationToken cancellationToken = default)
    {
        // Set steps from config
        await ConfigureAsync();
        
        // Start saga
        await base.StartSagaAsync(entityId, cancellationToken);
    }
}
        
/// <inheritdoc />
public abstract class ConfigurableSaga<TMetadata> : ConfigurableSaga
    where TMetadata : class
{
    /// <summary>
    /// Saga metadata.
    /// </summary>
    public TMetadata? Metadata { get; set; }

    /// <inheritdoc />
    protected ConfigurableSaga(ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) :
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }
    
    /// <summary>
    /// Start the saga.
    /// </summary>
    /// <param name="entity">Entity.</param>
    /// <param name="metadata">Saga metadata.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public virtual async Task StartSagaAsync(IEntity entity, TMetadata metadata,
        CancellationToken cancellationToken = default)
    {
        Metadata = metadata;
        Entity = entity;
        await StartSagaAsync(entity.Id, cancellationToken);
    }
}