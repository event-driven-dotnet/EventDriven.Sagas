using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Saga command.
/// </summary>
public abstract record SagaCommand(Guid? EntityId = default, IEntity? Entity = null) : ISagaCommand
{
    /// <inheritdoc />
    public string? Name { get; set; }
}

/// <summary>
/// Generic saga command.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
/// <typeparam name="TExpectedResult">Command expected result type.</typeparam>
public abstract record SagaCommand<TResult, TExpectedResult>(Guid? EntityId = default) :
    SagaCommand(EntityId), ISagaCommand<TResult, TExpectedResult>
{
    /// <inheritdoc />
    public TResult? Result { get; set; } = default!;

    /// <inheritdoc />
    public TExpectedResult ExpectedResult { get; set; } = default!;
}