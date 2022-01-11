using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Saga command handler.
/// </summary>
/// <typeparam name="TEntity">The type of entity.</typeparam>
/// <typeparam name="TCommand">The type of saga command.</typeparam>
public interface ISagaCommandHandler<TEntity, in TCommand>
    where TEntity : Entity
    where TCommand : class, ISagaCommand
{
    /// <summary>
    /// Command result processor.
    /// </summary>
    ICommandResultProcessor<TEntity>? CommandResultProcessor { get; set; }

    /// <summary>
    /// Handles a command.
    /// </summary>
    /// <param name="command">The command.</param>
    /// <returns>The command result.</returns>
    Task<CommandResult<TEntity>> Handle(TCommand command);
}
