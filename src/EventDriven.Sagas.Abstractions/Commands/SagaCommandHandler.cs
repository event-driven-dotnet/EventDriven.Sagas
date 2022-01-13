using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

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
    public abstract Task<CommandResult> HandleCommandAsync(TSagaCommand command);
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
    public abstract Task<CommandResult<TEntity>> HandleCommandAsync(TSagaCommand command);
}