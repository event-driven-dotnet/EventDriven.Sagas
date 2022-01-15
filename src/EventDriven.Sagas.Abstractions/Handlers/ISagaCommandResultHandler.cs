namespace EventDriven.Sagas.Abstractions.Handlers;

/// <summary>
/// Processor of a command result.
/// </summary>
public interface ISagaCommandResultHandler { }

/// <summary>
/// Processor of a command result.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
public interface ISagaCommandResultHandler<in TResult> : ISagaCommandResultHandler
{
    /// <summary>
    /// Process a command result.
    /// </summary>
    /// <param name="result">Command result.</param>
    /// <param name="compensating">True if compensating command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task HandleCommandResultAsync(TResult result, bool compensating);
}