using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;
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
        ICommandResultEvaluator commandResultEvaluator) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
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
            await SagaSnapshotRepository.PersistSagaSnapshotAsync(this);
    }
}

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public abstract class PersistableSaga<TEntity> :
    PersistableSaga,
    ICommandResultProcessor<TEntity>
    where TEntity : IEntity
{
    /// <inheritdoc />
    protected PersistableSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        ICommandResultEvaluator commandResultEvaluator) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
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

    /// <inheritdoc />
    public abstract Task ProcessCommandResultAsync(TEntity commandResult, bool compensating);
}