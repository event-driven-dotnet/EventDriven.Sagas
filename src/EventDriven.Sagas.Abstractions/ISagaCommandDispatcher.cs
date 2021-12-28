namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Dispatches saga commands.
/// </summary>
public interface ISagaCommandDispatcher
{
    /// <summary>
    /// Dispatch saga command.
    /// </summary>
    /// <param name="command">Saga command</param>
    /// <param name="compensating">True if compensating command.</param>
    /// <returns>Result of the asynchronous operation.</returns>
    Task DispatchAsync(ISagaCommand command, bool compensating);
}