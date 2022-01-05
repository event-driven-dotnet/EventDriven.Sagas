using EventDriven.DDD.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Saga command.
/// </summary>
public abstract record SagaCommand : Command, ISagaCommand
{
    /// <inheritdoc />
    protected SagaCommand(Guid entityId = default(Guid))
        : base(entityId)
    {
    }

    /// <inheritdoc />
    public string? Name { get; set; }
}

/// <summary>
/// Generic saga command.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
/// <typeparam name="TExpectedResult">Command expected result type.</typeparam>
public abstract record SagaCommand<TResult, TExpectedResult> :
    SagaCommand, ISagaCommand<TResult, TExpectedResult>
{
    /// <inheritdoc />
    public TResult Result { get; set; } = default!;

    /// <inheritdoc />
    public TExpectedResult ExpectedResult { get; set; } = default!;
}