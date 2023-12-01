using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Factories;
using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Pool.Abstractions.Pools;

/// <inheritdoc />
public class PersistableSagaPool<TSaga> : IPersistableSagaPool<TSaga>
    where TSaga : PersistableSaga
{
    private readonly bool _overrideLockCheck;
    private readonly ISagaFactory<TSaga> _sagaFactory;

    /// <summary>
    /// Saga Repository
    /// </summary>
    protected readonly IPersistableSagaRepository<TSaga> SagaRepository;

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
        SagaRepository = sagaRepository;
        _overrideLockCheck = overrideLockCheck;
    }

    /// <inheritdoc />
    public async Task<TSaga> GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity = null)
    {
        var newSaga = await ConfigureSagaAsync();
        var savedSaga =  await SagaRepository.GetAsync(id, newSaga);
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
        var savedSaga = await SagaRepository.CreateAsync(newSaga);
        return savedSaga;
    }
    
    async Task<Saga> ISagaPool.CreateSagaAsync() => await CreateSagaAsync();

    /// <inheritdoc />
    public async Task<TSaga> ReplaceSagaAsync(TSaga saga)
    {
        var newSaga = await ConfigureSagaAsync();
        return await SagaRepository.SaveAsync(saga, newSaga);
    }

    /// <inheritdoc />
    async Task<Saga> ISagaPool.ReplaceSagaAsync(Saga saga) => await ReplaceSagaAsync((TSaga)saga);

    /// <inheritdoc />
    public async Task RemoveSagaAsync(Guid id) => await SagaRepository.RemoveAsync(id);

    async Task ISagaPool.RemoveSagaAsync(Guid id) => await RemoveSagaAsync(id);
    
    /// <summary>
    /// Configure the reconstituted saga for operation.
    /// </summary>
    /// <returns>Configured saga.</returns>
    protected async Task<TSaga> ConfigureSagaAsync()
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


/// <summary>
/// Persistable saga with metadata.
/// </summary>
/// <typeparam name="TSaga">Persistable saga.</typeparam>
/// <typeparam name="TMetadata">Saga metadata.</typeparam>
public class PersistableSagaPool<TSaga,TMetadata>: PersistableSagaPool<TSaga>, IPersistableSagaPool<TSaga, TMetadata>
    where TSaga : PersistableSaga<TMetadata>
    where TMetadata : class
{
    /// <inheritdoc />
    public PersistableSagaPool(ISagaFactory<TSaga> sagaFactory, IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers, IPersistableSagaRepository<TSaga> sagaRepository, bool overrideLockCheck) : base(sagaFactory, commandResultDispatchers, sagaRepository, overrideLockCheck)
    {
    }

    /// <inheritdoc />
    public async Task<TSaga> CreateSagaAsync(TMetadata metaData)
    {
        var newSaga = await ConfigureSagaAsync();
        newSaga.Metadata = metaData;
        var savedSaga = await SagaRepository.CreateAsync(newSaga);
        return savedSaga;
    }
}