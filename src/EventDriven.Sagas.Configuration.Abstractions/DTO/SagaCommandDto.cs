namespace EventDriven.Sagas.Configuration.Abstractions.DTO;

/// <summary>
/// A command performed by a saga action.
/// </summary>
/// <typeparam name="TExpectedResult">Expected result type.</typeparam>
public record SagaCommandDto<TExpectedResult>
{
    /// <summary>
    /// Optional command name.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Command expected result.
    /// </summary>
    public TExpectedResult? ExpectedResult { get; set; }
}