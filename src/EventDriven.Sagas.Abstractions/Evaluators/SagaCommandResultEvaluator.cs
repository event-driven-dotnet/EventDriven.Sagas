namespace EventDriven.Sagas.Abstractions.Evaluators;

/// <summary>
/// Evaluator of a command result.
/// </summary>
/// <typeparam name="TResult">Result type.</typeparam>
/// <typeparam name="TExpectedResult">Expected result type.</typeparam>
public abstract class SagaCommandResultEvaluator<TResult, TExpectedResult>
    : ISagaCommandResultEvaluator<TResult, TExpectedResult>
{
    /// <inheritdoc />
    public abstract Task<bool> EvaluateCommandResultAsync(TResult? commandResult, TExpectedResult? expectedResult);

    /// <inheritdoc />
    public Type? SagaType { get; set; }
}

/// <summary>
/// Evaluator of a command result.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
/// <typeparam name="TResult">Result type.</typeparam>
/// <typeparam name="TExpectedResult">Expected result type.</typeparam>
public abstract class SagaCommandResultEvaluator<TSaga, TResult, TExpectedResult>
    : SagaCommandResultEvaluator<TResult, TExpectedResult>
    where TSaga : Saga
{
    /// <summary>
    /// Constructor.
    /// </summary>
    protected SagaCommandResultEvaluator()
    {
        SagaType = typeof(TSaga);
    }
}