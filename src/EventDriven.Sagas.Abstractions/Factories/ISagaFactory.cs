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
    /// <returns>Newly created saga.</returns>
    TSaga CreateSaga();
}