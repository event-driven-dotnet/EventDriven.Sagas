using EventDriven.Sagas.Abstractions.Pools;

namespace EventDriven.Sagas.Abstractions.Factories;

/// <summary>
/// Saga factory interface.
/// </summary>
public interface ISagaFactory<out TSaga>
    where TSaga : Saga
{
    /// <summary>
    /// Create a saga.
    /// </summary>
    /// <param name="sagaPool">Saga pool.</param>
    /// <param name="overrideLockCheck">True to override lock check.</param>
    /// <param name="enableSagaSnapshots">Enable saga snapshots.</param>
    /// <returns>Newly created saga.</returns>
    TSaga CreateSaga(ISagaPool sagaPool, bool overrideLockCheck, bool enableSagaSnapshots);
}