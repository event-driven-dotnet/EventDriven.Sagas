using System.Collections.Concurrent;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Factories;

namespace EventDriven.Sagas.Abstractions.Pools;

/// <inheritdoc />
public class SagaPool<TSaga> : ISagaPool<TSaga>
    where TSaga : Saga
{
    private readonly bool _overrideLockCheck;
    private readonly ISagaFactory<TSaga> _sagaFactory;
    private readonly ConcurrentDictionary<Guid, Saga> _sagaPool = new();

    /// <summary>
    /// Command result dispatchers.
    /// </summary>
    public IEnumerable<ISagaCommandResultDispatcher> SagaCommandResultDispatchers { get; }

    /// <summary>
    /// SagaPool constructor.
    /// </summary>
    /// <param name="sagaFactory">Saga factory.</param>
    /// <param name="commandResultDispatchers">Command result dispatchers.</param>
    /// <param name="overrideLockCheck">Override lock check.</param>
    public SagaPool(
        ISagaFactory<TSaga> sagaFactory,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        bool overrideLockCheck)
    {
        SagaCommandResultDispatchers = commandResultDispatchers;
        _overrideLockCheck = overrideLockCheck;
        _sagaFactory = sagaFactory;
    }

    /// <inheritdoc />
    public TSaga CreateSaga()
    {
        // Connect command result dispatchers to saga pool
        foreach (var commandResultDispatcher in SagaCommandResultDispatchers
            .Where(d => d.SagaType == null || d.SagaType == typeof(TSaga)))
            commandResultDispatcher.SagaPool = this;

        var saga = _sagaFactory.CreateSaga(_overrideLockCheck);
        _sagaPool.TryAdd(saga.Id, saga);
        return saga;
    }

    /// <inheritdoc />
    public void RemoveSaga(Guid sagaId)
    {
        _sagaPool.TryRemove(sagaId, out _);
    }

    /// <inheritdoc />
    public TSaga this[Guid index]
    {
        get => (TSaga)_sagaPool[index];
        set => _sagaPool[index] = value;
    }
}