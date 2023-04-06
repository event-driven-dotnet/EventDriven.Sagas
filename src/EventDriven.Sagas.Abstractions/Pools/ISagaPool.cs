using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Pools;

/// <summary>
/// Saga pool generic interface.
/// </summary>
public interface ISagaPool<TSaga> : ISagaPool
    where TSaga : Saga
{
    /// <summary>
    /// Get saga from the pool.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    /// <param name="retrieveEntity">Function to retrieve IEntity from storage.</param>
    /// <returns>Saga.</returns>
    new Task<TSaga> GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity = null);
    
    /// <summary>
    /// Create saga and add to the pool.
    /// </summary>
    /// <returns>Newly created saga.</returns>
    new Task<TSaga> CreateSagaAsync();

    /// <summary>
    /// Replace saga in the pool.
    /// </summary>
    /// <param name="saga">New saga</param>
    /// <returns>Saved saga.</returns>
    Task<TSaga> ReplaceSagaAsync(TSaga saga);
    
    /// <summary>
    /// Remove saga from the pool.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    new Task RemoveSagaAsync(Guid id);
}

/// <summary>
/// Saga pool interface.
/// </summary>
public interface ISagaPool
{
    /// <summary>
    /// Get saga from the pool.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    /// <param name="retrieveEntity">Function to retrieve IEntity from storage.</param>
    /// <returns>Saga.</returns>
    Task<Saga> GetSagaAsync(Guid id, Func<Guid, Task<IEntity?>>? retrieveEntity = null);
    
    /// <summary>
    /// Create saga and add to the pool.
    /// </summary>
    /// <returns>Newly created saga.</returns>
    Task<Saga> CreateSagaAsync();

    /// <summary>
    /// Replace saga in the pool.
    /// </summary>
    /// <param name="saga">New saga</param>
    /// <returns>Saved saga.</returns>
    Task<Saga> ReplaceSagaAsync(Saga saga);
    
    /// <summary>
    /// Remove saga from the pool.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    Task RemoveSagaAsync(Guid id);
}