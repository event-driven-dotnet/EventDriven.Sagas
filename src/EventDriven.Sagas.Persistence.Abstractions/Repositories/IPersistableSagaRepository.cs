using EventDriven.Sagas.Abstractions;

namespace EventDriven.Sagas.Persistence.Abstractions.Repositories;

/// <summary>
/// Repository interface for saga persistence.
/// </summary>
public interface IPersistableSagaRepository<TSaga>
    where TSaga : Saga
{
    /// <summary>
    /// Retrieve saga.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    /// <param name="newEntity">New saga.</param>
    /// <returns>Existing saga or null.</returns>
    Task<TSaga?> GetAsync(Guid id, TSaga newEntity);

    /// <summary>
    /// Create saga.
    /// </summary>
    /// <param name="newEntity">New saga.</param>
    /// <returns>Newly created saga.</returns>
    Task<TSaga> CreateAsync(TSaga newEntity);

    /// <summary>
    /// Save existing saga or add new saga.
    /// </summary>
    /// <param name="existingEntity">Existing saga.</param>
    /// <param name="newEntity">New saga.</param>
    /// <returns>Saved or added saga.</returns>
    Task<TSaga> SaveAsync(TSaga existingEntity, TSaga newEntity);

    /// <summary>
    /// Remove saga.
    /// </summary>
    /// <param name="id">Saga identifier.</param>
    Task RemoveAsync(Guid id);
}