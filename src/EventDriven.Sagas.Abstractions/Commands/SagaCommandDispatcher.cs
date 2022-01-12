using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <inheritdoc />
public abstract class SagaCommandDispatcher : ISagaCommandDispatcher
{
    /// <inheritdoc />
    public abstract Task DispatchAsync(SagaCommand command, bool compensating);
}

/// <summary>
/// Dispatches saga commands.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TSagaCommand">Saga command type.</typeparam>
public abstract class SagaCommandDispatcher<TEntity, TSagaCommand> :
    SagaCommandDispatcher,
    ISagaCommandDispatcher<TEntity, TSagaCommand>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <inheritdoc />
    public ISagaCommandHandler<TEntity, TSagaCommand> SagaCommandHandler { get; set; } = null!;
}
