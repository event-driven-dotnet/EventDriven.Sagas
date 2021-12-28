namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Processor of a command result.
/// </summary>
public interface ICommandResultProcessor<in TResult>
{
    /// <summary>
    /// Process a command result.
    /// </summary>
    /// <param name="commandResult">Command result</param>
    /// <param name="compensating">True if compensating command.</param>
    /// <returns>Result of the asynchronous operation.</returns>
    Task ProcessCommandResultAsync(TResult commandResult, bool compensating);
}