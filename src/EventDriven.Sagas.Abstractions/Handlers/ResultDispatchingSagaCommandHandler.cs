using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;

namespace EventDriven.Sagas.Abstractions.Handlers;

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
    public Type? SagaType { get; set; }

    /// <inheritdoc />
    public abstract Task HandleCommandAsync(TSagaCommand command);

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
/// <typeparam name="TSaga">Saga type.</typeparam>
/// <typeparam name="TSagaCommand">Command type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingSagaCommandHandler<TSaga, TSagaCommand, TResult> :
    ResultDispatchingSagaCommandHandler<TSagaCommand, TResult>
    where TSaga : Saga
    where TSagaCommand : class, ISagaCommand
{
    /// <summary>
    /// Constructor.
    /// </summary>
    protected ResultDispatchingSagaCommandHandler()
    {
        SagaType = typeof(TSaga);
    }
}