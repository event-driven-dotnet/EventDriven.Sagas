using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Abstractions.Pools;

/// <inheritdoc />
public class PersistableSagaPool<TSaga> : ISagaPool<TSaga>
    where TSaga : Saga
{
    private readonly bool _overrideLockCheck;
    private readonly ISagaFactory<TSaga> _sagaFactory;
    private readonly IPersistableSagaRepository<TSaga> _sagaRepository;

    /// <summary>
    /// Command result dispatchers.
    /// </summary>
    public IEnumerable<ISagaCommandResultDispatcher> SagaCommandResultDispatchers { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaRepository">Saga repository.</param>
    /// <param name="sagaFactory">Saga factory.</param>
    /// <param name="commandResultDispatchers">Command result dispatchers.</param>
    /// <param name="overrideLockCheck">Override lock check.</param>
    public PersistableSagaPool(
        ISagaFactory<TSaga> sagaFactory,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        IPersistableSagaRepository<TSaga> sagaRepository,
        bool overrideLockCheck)
    {
        SagaCommandResultDispatchers = commandResultDispatchers;
        _sagaFactory = sagaFactory;
        _sagaRepository = sagaRepository;
        _overrideLockCheck = overrideLockCheck;
    }

    /// <inheritdoc />
    public async Task<TSaga> GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity = null)
    {
        var newSaga = await ConfigureSagaAsync();
        var savedSaga =  await _sagaRepository.GetAsync(id, newSaga);
        if (savedSaga is null)
            throw new KeyNotFoundException();
        if (retrieveEntity is not null)
            savedSaga.Entity = await retrieveEntity.Invoke(savedSaga.EntityId);
        return savedSaga;
    }

    async Task<Saga> ISagaPool.GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity) => 
        await GetSagaAsync(id, retrieveEntity);

    /// <inheritdoc />
    public async Task<TSaga> CreateSagaAsync()
    {
        var newSaga = await ConfigureSagaAsync();
        var savedSaga = await _sagaRepository.CreateAsync(newSaga);
        return savedSaga;
    }
    
    async Task<Saga> ISagaPool.CreateSagaAsync() => await CreateSagaAsync();

    /// <inheritdoc />
    public async Task<TSaga> ReplaceSagaAsync(TSaga saga)
    {
        var newSaga = await ConfigureSagaAsync();
        return await _sagaRepository.SaveAsync(saga, newSaga);
    }

    /// <inheritdoc />
    async Task<Saga> ISagaPool.ReplaceSagaAsync(Saga saga) => await ReplaceSagaAsync((TSaga)saga);

    /// <inheritdoc />
    public async Task RemoveSagaAsync(Guid id) => await _sagaRepository.RemoveAsync(id);

    async Task ISagaPool.RemoveSagaAsync(Guid id) => await RemoveSagaAsync(id);
    
    private async Task<TSaga> ConfigureSagaAsync()
    {
        foreach (var commandResultDispatcher in SagaCommandResultDispatchers
            .Where(d => d.SagaType == null || d.SagaType == typeof(TSaga)))
            commandResultDispatcher.SagaPool = this;

        var saga = _sagaFactory.CreateSaga(this, _overrideLockCheck);
        if (saga is ConfigurableSaga configSaga)
            await configSaga.ConfigureAsync();
        return saga;
    }
}