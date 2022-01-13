namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Saga command result dispatcher.
/// </summary>
public interface ISagaCommandResultDispatcher
{
    /// <summary>
    /// Saga command result handler.
    /// </summary>
    public ISagaCommandResultHandler SagaCommandResultHandler { get; set; }
}

/// <summary>
/// 
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
public interface ISagaCommandResultDispatcher<in TResult> : ISagaCommandResultDispatcher
{
    /// <summary>
    /// Dispatch result.
    /// </summary>
    /// <param name="commandResult">Command result.</param>
    /// <param name="compensating">True if compensating command.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DispatchCommandResultAsync(TResult commandResult, bool compensating);
}