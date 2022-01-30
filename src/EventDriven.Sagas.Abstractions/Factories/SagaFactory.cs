using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;

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
    /// <param name="checkLockCommandHandlers">Saga check lock command handler.</param>
    public SagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> sagaCommandResultEvaluators,
        IEnumerable<ISagaCommandResultDispatcher> commandResultDispatchers,
        IEnumerable<ICheckSagaLockCommandHandler> checkLockCommandHandlers)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        SagaCommandResultEvaluators = sagaCommandResultEvaluators;
        SagaCommandResultDispatchers = commandResultDispatchers;
        CheckLockCommandHandlers = checkLockCommandHandlers;
    }

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    public virtual ISagaCommandDispatcher SagaCommandDispatcher { get; }

    /// <summary>
    /// Command result evaluators.
    /// </summary>
    public virtual IEnumerable<ISagaCommandResultEvaluator> SagaCommandResultEvaluators { get; }

    /// <summary>
    /// Check lock command handlers.
    /// </summary>
    public virtual IEnumerable<ICheckSagaLockCommandHandler> CheckLockCommandHandlers { get; }

    /// <summary>
    /// Command result dispatchers.
    /// </summary>
    protected IEnumerable<ISagaCommandResultDispatcher> SagaCommandResultDispatchers { get; }

    /// <inheritdoc />
    public virtual TSaga CreateSaga(bool overrideLock)
    {
        var saga = (TSaga?)Activator.CreateInstance(
            typeof(TSaga), SagaCommandDispatcher, SagaCommandResultEvaluators);
        if (saga == null)
            throw new Exception($"Unable to create instance of {typeof(TSaga).Name}");
        foreach (var commandResultDispatcher in SagaCommandResultDispatchers
            .Where(d => d.SagaType == null || d.SagaType == typeof(TSaga)))
            commandResultDispatcher.SagaCommandResultHandler = saga;
        var checkLockHandler = CheckLockCommandHandlers.FirstOrDefault(
            h => h.SagaType == typeof(TSaga));
        saga.OverrideLockCheck = overrideLock;
        saga.CheckLockCommandHandler = checkLockHandler;
        saga.Id = Guid.NewGuid();
        return saga;
    }
}