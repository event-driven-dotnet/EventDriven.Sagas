namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Saga command.
/// </summary>
public interface ISagaCommand
{
    /// <summary>
    /// Command name.
    /// </summary>
    public string? Name { get; set; }
}

/// <summary>
/// Generic saga command.
/// </summary>
/// <typeparam name="TPayload">Payload type.</typeparam>
/// <typeparam name="TResult">Expected result</typeparam>
public interface ISagaCommand<TPayload, TResult> : ISagaCommand
{
    /// <summary>
    /// Command payload
    /// </summary>
    public TPayload Payload { get; set; }

    /// <summary>
    /// Command expected result.
    /// </summary>
    public TResult ExpectedResult { get; set; }
}