using EventDriven.Sagas.Abstractions.Pools;
using EventDriven.Sagas.Persistence.Abstractions;

namespace EventDriven.Sagas.Persistence.Pool.Abstractions.Pools;

/// <inheritdoc />
public interface IPersistableSagaPool<TSaga>: ISagaPool<TSaga>
    where TSaga : PersistableSaga
{
}

/// <inheritdoc />
public interface IPersistableSagaPool<TSaga, TMetaData>: IPersistableSagaPool<TSaga>
    where TSaga : PersistableSaga
    where TMetaData : class
{
    /// <summary>
    /// Create saga and add to the pool.
    /// </summary>
    /// <param name="metaData">Saga metadata.</param>
    /// <returns>Newly created saga.</returns>
    Task<TSaga> CreateSagaAsync(TMetaData metaData);
}