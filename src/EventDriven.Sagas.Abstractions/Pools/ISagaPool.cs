namespace EventDriven.Sagas.Abstractions.Pools;

/// <summary>
/// Saga pool generic interface.
/// </summary>
public interface ISagaPool<TSaga> : ISagaPool
    where TSaga : Saga
{
    /// <summary>
    /// Create saga and add to the pool.
    /// </summary>
    /// <returns>Newly created saga.</returns>
    TSaga CreateSaga();
    
    /// <summary>
    /// Saga pool indexer.
    /// </summary>
    /// <param name="index">Saga pool index.</param>
    TSaga this[Guid index] { get; set; }
}

/// <summary>
/// Saga pool interface.
/// </summary>
public interface ISagaPool
{
    /// <summary>
    /// Remove saga from the pool.
    /// </summary>
    /// <param name="sagaId">Id of saga to remove.</param>
    void RemoveSaga(Guid sagaId);
}