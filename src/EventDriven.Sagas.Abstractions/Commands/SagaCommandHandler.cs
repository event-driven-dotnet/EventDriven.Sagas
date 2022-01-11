using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <inheritdoc />
public abstract class SagaCommandHandler<TEntity, TCommand> :
    ISagaCommandHandler<TEntity, TCommand>
    where TEntity : Entity
    where TCommand : class, ISagaCommand
{
    /// <summary>
    /// Command result processor.
    /// </summary>
    public virtual ICommandResultProcessor<TEntity>? CommandResultProcessor { get; set; } = null!;

    /// <inheritdoc />
    public abstract Task<CommandResult<TEntity>> Handle(TCommand command);
}