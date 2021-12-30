namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// The state of an action.
/// </summary>
public enum ActionState
{
    /// <summary>
    /// Initial action state.
    /// </summary>
    Initial,

    /// <summary>
    /// Running action state.
    /// </summary>
    Running,

    /// <summary>
    /// Succeeded action state.
    /// </summary>
    Succeeded,

    /// <summary>
    /// Failed action state.
    /// </summary>
    Failed,

    /// <summary>
    /// Timed out action state.
    /// </summary>
    TimedOut,

    /// <summary>
    /// Cancelled action state.
    /// </summary>
    Cancelled
}