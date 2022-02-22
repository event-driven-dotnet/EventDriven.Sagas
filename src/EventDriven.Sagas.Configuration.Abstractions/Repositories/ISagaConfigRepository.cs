namespace EventDriven.Sagas.Configuration.Abstractions.Repositories;

/// <summary>
/// Repository interface for saga configuration.
/// </summary>
public interface ISagaConfigRepository
{
    /// <summary>
    /// Retrieve a saga configuration.
    /// </summary>
    /// <param name="id">Saga config id.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the saga configuration.
    /// </returns>
    Task<SagaConfiguration?> GetAsync(Guid id);

    /// <summary>
    /// Add a new saga configuration.
    /// </summary>
    /// <param name="entity">A new saga configuration.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the added saga configuration.
    /// </returns>
    Task<SagaConfiguration?> AddAsync(SagaConfiguration entity);

    /// <summary>
    /// Update an existing saga configuration.
    /// </summary>
    /// <param name="entity">An existing saga configuration.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the updated saga configuration.
    /// </returns>
    Task<SagaConfiguration?> UpdateAsync(SagaConfiguration entity);

    /// <summary>
    /// Remove an existing saga configuration.
    /// </summary>
    /// <param name="id">Saga configuration identifier.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the number of items deleted.
    /// </returns>
    Task<int> RemoveAsync(Guid id);
}