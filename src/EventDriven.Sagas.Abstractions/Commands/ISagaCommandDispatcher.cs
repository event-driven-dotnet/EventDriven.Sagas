using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Dispatches saga commands.
/// </summary>
public interface ISagaCommandDispatcher
{
    /// <summary>
    /// Dispatch saga command.
    /// </summary>
    /// <param name="command">Saga command</param>
    /// <param name="compensating">True if compensating command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchAsync(SagaCommand command, bool compensating);
}

/// <summary>
/// Dispatches saga commands.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TSagaCommand">Saga command type.</typeparam>
public interface ISagaCommandDispatcher<TEntity, TSagaCommand> : ISagaCommandDispatcher
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Saga command handler.
    /// </summary>
    public ISagaCommandHandler<TEntity, TSagaCommand> SagaCommandHandler { get; set; }
}