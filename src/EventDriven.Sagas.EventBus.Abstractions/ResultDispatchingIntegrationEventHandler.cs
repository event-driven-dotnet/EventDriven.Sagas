using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;

namespace EventDriven.Sagas.EventBus.Abstractions;

/// <summary>
/// Integration event handler that can dispatch command results.
/// </summary>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
public abstract class ResultDispatchingIntegrationEventHandler<TIntegrationEvent> : 
    IntegrationEventHandler<TIntegrationEvent>
    where TIntegrationEvent : IIntegrationEvent
{
}

/// <summary>
/// Integration event handler that can dispatch command results.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingIntegrationEventHandler<TSaga, TIntegrationEvent, TResult> :
    ResultDispatchingIntegrationEventHandler<TIntegrationEvent>,
    ISagaCommandResultDispatcher<TResult>
    where TSaga : Saga
    where TIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Saga pool.
    /// </summary>
    public ISagaPool SagaPool { get; set; } = null!;

    /// <inheritdoc />
    public Type? SagaType { get; set; }

    /// <inheritdoc />
    public async Task DispatchCommandResultAsync(TResult commandResult, bool compensating, Guid sagaId)
    {
        // Use Saga Pool to get saga
        var sagaPool = (SagaPool<TSaga>)SagaPool;
        var saga = sagaPool[sagaId];
        if (saga is ISagaCommandResultHandler<TResult> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
}