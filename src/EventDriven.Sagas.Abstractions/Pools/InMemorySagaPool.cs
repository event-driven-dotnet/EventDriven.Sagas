using System.Collections.Concurrent;
using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Factories;

namespace EventDriven.Sagas.Abstractions.Pools;

/// <inheritdoc />
public class InMemorySagaPool<TSaga> : ISagaPool<TSaga>
    where TSaga : Saga
{
    private readonly bool _overrideLockCheck;
    private readonly bool _enableSagaSnapshots;
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
    /// <param name="enableSagaSnapshots">Enable saga snapshots.</param>
    public InMemorySagaPool(
        ISagaFactory<TSaga> sagaFactory,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        bool overrideLockCheck, bool enableSagaSnapshots)
    {
        SagaCommandResultDispatchers = commandResultDispatchers;
        _overrideLockCheck = overrideLockCheck;
        _enableSagaSnapshots = enableSagaSnapshots;
        _sagaFactory = sagaFactory;
    }

    /// <inheritdoc />
    public Task<TSaga> GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity = null)
    {
        var success = _sagaPool.TryGetValue(id, out var saga);
        if (!success) throw new KeyNotFoundException();
        return Task.FromResult((TSaga)saga!);
    }

    async Task<Saga> ISagaPool.GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity) =>
        await GetSagaAsync(id, retrieveEntity);

    /// <inheritdoc />
    public Task<TSaga> CreateSagaAsync()
    {
        // Connect command result dispatchers to saga pool
        foreach (var commandResultDispatcher in SagaCommandResultDispatchers
            .Where(d => d.SagaType == null || d.SagaType == typeof(TSaga)))
            commandResultDispatcher.SagaPool = this;
        
        var saga = _sagaFactory.CreateSaga(this, _overrideLockCheck, _enableSagaSnapshots);
        _sagaPool.TryAdd(saga.Id, saga);
        return Task.FromResult(saga);
    }

    async Task<Saga> ISagaPool.CreateSagaAsync() => await CreateSagaAsync();

    /// <inheritdoc />
    public Task<TSaga> ReplaceSagaAsync(TSaga saga)
    {
        var success = _sagaPool.TryGetValue(saga.Id, out var existing);
        if (success && existing is not null)
            _sagaPool.TryUpdate(saga.Id, saga, existing);
        else
            _sagaPool.TryAdd(saga.Id, saga);
        return Task.FromResult(saga);
    }

    /// <inheritdoc />
    async Task<Saga> ISagaPool.ReplaceSagaAsync(Saga saga) => await ReplaceSagaAsync((TSaga)saga);

    /// <inheritdoc />
    public Task RemoveSagaAsync(Guid id)
    {
        _sagaPool.TryRemove(id, out _);
        return Task.CompletedTask;
    }

    async Task ISagaPool.RemoveSagaAsync(Guid id) => await RemoveSagaAsync(id);
}