using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Saga command.
/// </summary>
public interface ISagaCommand
{
    /// <summary>
    /// Optional command name.
    /// </summary>
    public string? Name { get; set; }
    
    /// <summary>
    /// Represents the ID of the entity the command is in reference to.
    /// </summary>
    Guid? EntityId { get; }

    /// <summary>
    /// The entity the command is in reference to.
    /// </summary>
    public IEntity? Entity { get; }
}

/// <summary>
/// Generic saga command.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
/// <typeparam name="TExpectedResult">Command expected result type.</typeparam>
public interface ISagaCommand<TResult, TExpectedResult> : ISagaCommand
{
    /// <summary>
    /// Command result.
    /// </summary>
    public TResult? Result { get; set; }

    /// <summary>
    /// Command expected result.
    /// </summary>
    public TExpectedResult ExpectedResult { get; set; }
}

/// <summary>
/// Generic saga command.
/// </summary>
/// <typeparam name="TResult">Command result type.</typeparam>
public interface ISagaCommand<TResult> : ISagaCommand
{
    /// <summary>
    /// Command result.
    /// </summary>
    public TResult Result { get; set; }

    /// <summary>
    /// Command expected result.
    /// </summary>
    public TResult ExpectedResult { get; set; }
}