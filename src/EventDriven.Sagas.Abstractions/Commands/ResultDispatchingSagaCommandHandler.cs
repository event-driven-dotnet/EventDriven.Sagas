using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Command handler that can dispatch command results.
/// </summary>
/// <typeparam name="TSagaCommand">Command type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingSagaCommandHandler<TSagaCommand, TResult> :
    ISagaCommandHandler<TSagaCommand>,
    ISagaCommandResultDispatcher<TResult>
    where TSagaCommand : class, ISagaCommand
{
    /// <inheritdoc />
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    /// <inheritdoc />
    public abstract Task<CommandResult> HandleCommandAsync(TSagaCommand command);

    /// <inheritdoc />
    public async Task DispatchCommandResultAsync(TResult commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<TResult> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
}

/// <summary>
/// Command handler that can dispatch command results.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
/// <typeparam name="TSagaCommand">Command type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingSagaCommandHandler<TEntity, TSagaCommand, TResult> :
    ISagaCommandHandler<TEntity, TSagaCommand>,
    ISagaCommandResultDispatcher<TResult>
    where TEntity : Entity
    where TSagaCommand : class, ISagaCommand
{
    /// <inheritdoc />
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    /// <inheritdoc />
    public abstract Task<CommandResult<TEntity>> HandleCommandAsync(TSagaCommand command);

    /// <inheritdoc />
    public async Task DispatchCommandResultAsync(TResult commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<TResult> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
}