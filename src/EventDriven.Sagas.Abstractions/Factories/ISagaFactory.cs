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
    /// <param name="overrideLockCheck">True to override lock check.</param>
    /// <returns>Newly created saga.</returns>
    TSaga CreateSaga(bool overrideLockCheck);
}