using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Factories;

/// <summary>
/// Factory for creating saga instances.
/// </summary>
/// <typeparam name="TSaga">Saga type.</typeparam>
public class SagaFactory<TSaga> : ISagaFactory<TSaga>
    where TSaga : Saga, ISagaCommandResultHandler
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sagaCommandDispatcher">Saga command dispatcher.</param>
    /// <param name="sagaCommandResultEvaluators">Saga command result evaluator.</param>
    /// <param name="commandResultDispatchers">Command result dispatchers</param>
    public SagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> sagaCommandResultEvaluators,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        SagaCommandResultEvaluators = sagaCommandResultEvaluators;
        SagaCommandResultDispatchers = commandResultDispatchers;
    }

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    public virtual ISagaCommandDispatcher SagaCommandDispatcher { get; }

    /// <summary>
    /// Command result evaluator.
    /// </summary>
    public virtual IEnumerable<ISagaCommandResultEvaluator> SagaCommandResultEvaluators { get; }

    /// <summary>
    /// Command result dispatchers.
    /// </summary>
    protected IEnumerable<ISagaCommandResultDispatcher> SagaCommandResultDispatchers { get; }

    /// <inheritdoc />
    public virtual TSaga CreateSaga()
    {
        var saga = (TSaga?)Activator.CreateInstance(
            typeof(TSaga), SagaCommandDispatcher, SagaCommandResultEvaluators);
        if (saga == null)
            throw new Exception($"Unable to create instance of {typeof(TSaga).Name}");
        foreach (var commandResultDispatcher in SagaCommandResultDispatchers
            .Where(d => d.SagaType == null || d.SagaType == typeof(TSaga)))
            commandResultDispatcher.SagaCommandResultHandler = saga;
        return saga;
    }
}