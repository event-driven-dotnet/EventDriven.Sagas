namespace EventDriven.Sagas.Abstractions.Commands;

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
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchAsync(ISagaCommand command, bool compensating);
}