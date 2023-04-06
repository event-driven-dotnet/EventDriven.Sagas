using EventDriven.DDD.Abstractions.Entities;
using EventDriven.Sagas.Abstractions.Pools;

namespace EventDriven.Sagas.Abstractions.Dispatchers;

/// <summary>
/// Saga command result dispatcher.
/// </summary>
public interface ISagaCommandResultDispatcher
{
    /// <summary>
    /// Saga pool.
    /// </summary>
    public ISagaPool SagaPool { get; set; }

    /// <summary>
    /// Saga type.
    /// </summary>
    public Type? SagaType { get; set; }
}

/// <summary>
/// Saga command result dispatcher.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public interface ISagaCommandResultDispatcher<in TResult> : ISagaCommandResultDispatcher
{
    /// <summary>
    /// Dispatch result.
    /// </summary>
    /// <param name="commandResult">Command result.</param>
    /// <param name="compensating">True if compensating command.</param>
    /// <param name="sagaId">Saga identifier.</param>
    /// <param name="entity">Saga entity.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchCommandResultAsync(TResult commandResult, bool compensating, Guid sagaId, IEntity? entity);
}