using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Repositories;

/// <summary>
/// Repository interface for saga history.
/// </summary>
public interface ISagaSnapshotRepository
{
    /// <summary>
    /// Retrieve a saga.
    /// </summary>
    /// <param name="id">Saga id.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the saga.
    /// </returns>
    Task<Saga?> GetSagaAsync(Guid id);

    /// <summary>
    /// Add a new saga.
    /// </summary>
    /// <param name="entity">A new saga.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the added saga.
    /// </returns>
    Task<Saga?> AddSagaAsync(Saga entity);

    /// <summary>
    /// Update an existing saga.
    /// </summary>
    /// <param name="entity">An existing saga.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the updated saga.
    /// </returns>
    Task<Saga?> UpdateSagaAsync(Saga entity);

    /// <summary>
    /// Remove an existing saga.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of items deleted.
    /// </returns>
    Task<int> RemoveSagaAsync(Guid id);
}