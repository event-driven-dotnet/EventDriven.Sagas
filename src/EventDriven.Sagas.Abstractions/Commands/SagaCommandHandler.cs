using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <inheritdoc />
public abstract class SagaCommandHandler<TEntity, TSagaCommand> :
    ISagaCommandHandler<TEntity, TSagaCommand>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Command result processor.
    /// </summary>
    public virtual ICommandResultProcessor<TEntity>? CommandResultProcessor { get; set; } = null!;

    /// <inheritdoc />
    public abstract Task<CommandResult<TEntity>> Handle(TSagaCommand command);
}