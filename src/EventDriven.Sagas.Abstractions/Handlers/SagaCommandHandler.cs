using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Handlers;

/// <inheritdoc />
public abstract class SagaCommandHandler<TSagaCommand> :
    ISagaCommandHandler<TSagaCommand>
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Command result processor.
    /// </summary>
    public virtual ISagaCommandResultHandler? CommandResultHandler { get; set; } = null!;

    /// <inheritdoc />
    public abstract Task HandleCommandAsync(TSagaCommand command);
}

/// <inheritdoc />
public abstract class SagaCommandHandler<TEntity, TSagaCommand> :
    ISagaCommandHandler<TEntity, TSagaCommand>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Command result processor.
    /// </summary>
    public virtual ISagaCommandResultHandler<TEntity>? CommandResultHandler { get; set; } = null!;

    /// <inheritdoc />
    public abstract Task<TEntity> HandleCommandAsync(TSagaCommand command);
}