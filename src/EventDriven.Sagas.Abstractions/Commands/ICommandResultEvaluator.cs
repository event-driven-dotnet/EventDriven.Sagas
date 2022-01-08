using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Commands;

/// <summary>
/// Evaluator of a command result.
/// </summary>
public interface ICommandResultEvaluator
{
    /// <summary>
    /// Saga state information.
    /// </summary>
    public string? SagaStateInfo { get; set; }
    
    /// <summary>
    /// Evaluate a step result.
    /// </summary>
    /// <param name="step">Saga step.</param>
    /// <param name="compensating">True if compensating step.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a boolean that is true if step completed successfully.
    /// </returns>
    Task<bool> EvaluateStepResultAsync(SagaStep step, bool compensating, CancellationToken cancellationToken);
}

/// <summary>
/// Evaluator of a command result.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
/// <typeparam name="TExpectedResult">Expected result type.</typeparam>
public interface ICommandResultEvaluator<in TResult, in TExpectedResult> : ICommandResultEvaluator
{
    /// <summary>
    /// Evaluate a command result.
    /// </summary>
    /// <param name="commandResult">Command result.</param>
    /// <param name="expectedResult">Expected result.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a boolean.
    /// </returns>
    Task<bool> EvaluateCommandResultAsync(TResult commandResult, TExpectedResult expectedResult);
}