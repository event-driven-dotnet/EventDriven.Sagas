namespace EventDriven.Sagas.Persistence.Abstractions.Repositories;

/// <summary>
/// Repository interface for saga history.
/// </summary>
public interface ISagaSnapshotRepository
{
    /// <summary>
    /// Retrieve a saga.
    /// </summary>
    /// <param name="id">Saga id.</param>
    /// <param name="entity">A persistable saga.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task RetrieveAsync(Guid id, PersistableSaga entity);

    /// <summary>
    /// Add a persistable saga.
    /// </summary>
    /// <param name="entity">A persistable saga.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    Task PersistAsync(PersistableSaga entity);
}