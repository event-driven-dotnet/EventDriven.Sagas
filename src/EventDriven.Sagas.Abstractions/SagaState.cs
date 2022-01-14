namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// The state of a saga.
/// </summary>
public enum SagaState
{
    /// <summary>
    /// Initial saga state.
    /// </summary>
    Initial,

    /// <summary>
    /// Executing saga state.
    /// </summary>
    Executing,

    /// <summary>
    /// Executed saga state.
    /// </summary>
    Executed,

    /// <summary>
    /// Compensating saga state.
    /// </summary>
    Compensating,

    /// <summary>
    /// Compensated saga state.
    /// </summary>
    Compensated
}