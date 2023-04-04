using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class PersistableSaga : ConfigurableSaga
{
    /// <inheritdoc />
    protected PersistableSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators,
        ISagaPool sagaPool) : 
        base(sagaCommandDispatcher, commandResultEvaluators, sagaPool)
    {
    }

    /// <summary>
    /// Saga snapshot repository.
    /// </summary>
    public ISagaSnapshotRepository? SagaSnapshotRepository { get; set; }

    /// <summary>
    /// Persist saga.
    /// </summary>
    protected virtual async Task PersistAsync()
    {
        if (SagaSnapshotRepository != null)
            await SagaSnapshotRepository.PersistAsync(this);
    }
}

/// <inheritdoc />
public abstract class PersistableSaga<TMetadata> : PersistableSaga
    where TMetadata : class
{
    /// <summary>
    /// Saga metadata.
    /// </summary>
    public TMetadata? Metadata { get; set; }

    /// <inheritdoc />
    protected PersistableSaga(ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators,
        ISagaPool sagaPool) :
        base(sagaCommandDispatcher, commandResultEvaluators, sagaPool)
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