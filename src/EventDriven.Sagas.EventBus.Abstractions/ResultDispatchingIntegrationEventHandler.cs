using EventDriven.EventBus.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

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