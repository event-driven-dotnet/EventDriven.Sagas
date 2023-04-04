namespace EventDriven.Sagas.Abstractions.Pools;

/// <summary>
/// Saga pool generic interface.
/// </summary>
public interface ISagaPool<out TSaga> : ISagaPool
    where TSaga : Saga
{
    /// <summary>
    /// Create saga and add to the pool.
    /// </summary>
    /// <returns>Newly created saga.</returns>
    new TSaga CreateSaga();
    
    /// <summary>
    /// Saga pool indexer.
    /// </summary>
    /// <param name="index">Saga pool index.</param>
    new TSaga this[Guid index] { get; }
}

/// <summary>
/// Saga pool interface.
/// </summary>
public interface ISagaPool
{
    /// <summary>
    /// Create saga and add to the pool.
    /// </summary>
    /// <returns>Newly created saga.</returns>
    Saga CreateSaga();
    
    /// <summary>
    /// Remove saga from the pool.
    /// </summary>
    /// <param name="sagaId">Id of saga to remove.</param>
    void RemoveSaga(Guid sagaId);
    
    /// <summary>
    /// Saga pool indexer.
    /// </summary>
    /// <param name="index">Saga pool index.</param>
    Saga this[Guid index] { get; }
}