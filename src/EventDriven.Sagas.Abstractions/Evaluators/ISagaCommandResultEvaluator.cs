namespace EventDriven.Sagas.Abstractions.Evaluators;

/// <summary>
/// Evaluator of a command result.
/// </summary>
public interface ISagaCommandResultEvaluator
{
    /// <summary>
    /// Saga type.
    /// </summary>
    public Type? SagaType { get; set; }
}

/// <summary>
/// Evaluator of a command result.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
/// <typeparam name="TExpectedResult">Expected result type.</typeparam>
public interface ISagaCommandResultEvaluator<in TResult, in TExpectedResult> : ISagaCommandResultEvaluator
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
    Task<bool> EvaluateCommandResultAsync(TResult? commandResult, TExpectedResult? expectedResult);
}