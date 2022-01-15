using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.EventBus.Abstractions;

/// <summary>
/// Integration event handler that can dispatch command results.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingIntegrationEventHandler<TIntegrationEvent, TResult> : 
    IntegrationEventHandler<TIntegrationEvent>,
    ISagaCommandResultDispatcher<TResult>
    where TIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Saga type.
    /// </summary>
    public Type? SagaType { get; set; }

    /// <summary>
    /// Saga command result handler.
    /// </summary>
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

    /// <inheritdoc />
    public async Task DispatchCommandResultAsync(TResult commandResult, bool compensating)
    {
        if (SagaCommandResultHandler is ISagaCommandResultHandler<TResult> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
}

/// <summary>
/// Integration event handler that can dispatch command results.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingIntegrationEventHandler<TSaga, TIntegrationEvent, TResult> :
    ResultDispatchingIntegrationEventHandler<TIntegrationEvent, TResult>
    where TSaga : Saga
    where TIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Constructor.
    /// </summary>
    protected ResultDispatchingIntegrationEventHandler()
    {
        SagaType = typeof(TSaga);
    }
}