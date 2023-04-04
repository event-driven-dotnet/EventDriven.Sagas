using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Abstractions.Pools;

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
    /// <param name="checkLockCommandHandlers">Saga check lock command handler.</param>
    public SagaFactory(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> sagaCommandResultEvaluators,
        IEnumerable<ICheckSagaLockCommandHandler> checkLockCommandHandlers)
    {
        SagaCommandDispatcher = sagaCommandDispatcher;
        SagaCommandResultEvaluators = sagaCommandResultEvaluators;
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

    /// <inheritdoc />
    public virtual TSaga CreateSaga(ISagaPool sagaPool, bool overrideLock)
    {
        var saga = (TSaga?)Activator.CreateInstance(
            typeof(TSaga), SagaCommandDispatcher, SagaCommandResultEvaluators, sagaPool);
        if (saga == null)
            throw new Exception($"Unable to create instance of {typeof(TSaga).Name}");
        var checkLockHandler = CheckLockCommandHandlers.FirstOrDefault(
            h => h.SagaType == typeof(TSaga));
        saga.OverrideLockCheck = overrideLock;
        saga.CheckLockCommandHandler = checkLockHandler;
        saga.Id = Guid.NewGuid();
        return saga;
    }
}