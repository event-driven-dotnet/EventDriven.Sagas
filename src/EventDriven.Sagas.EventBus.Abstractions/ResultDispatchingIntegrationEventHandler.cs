using EventDriven.DDD.Abstractions.Entities;
using EventDriven.EventBus.Abstractions;
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
/// <typeparam name="TIntegrationEvent">Integration event type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
public abstract class ResultDispatchingIntegrationEventHandler<TIntegrationEvent, TResult> :
    ResultDispatchingIntegrationEventHandler<TIntegrationEvent>,
    ISagaCommandResultDispatcher<TResult>
    where TIntegrationEvent : IIntegrationEvent
{
    /// <summary>
    /// Saga pool.
    /// </summary>
    public ISagaPool SagaPool { get; set; } = null!;

    /// <inheritdoc />
    public Type? SagaType { get; set; }

    /// <inheritdoc />
    public async Task DispatchCommandResultAsync(TResult commandResult, bool compensating, Guid sagaId, IEntity? entity)
    {
        // Use Saga Pool to get saga
        var saga = await SagaPool.GetSagaAsync(sagaId);
        saga.Entity = entity;
        if (saga is ISagaCommandResultHandler<TResult> handler)
            await handler.HandleCommandResultAsync(commandResult, compensating);
    }
}