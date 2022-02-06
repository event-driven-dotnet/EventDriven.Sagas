using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Persistence.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class PersistableSaga : ConfigurableSaga
{
    /// <inheritdoc />
    protected PersistableSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) : 
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }

    /// <summary>
    /// Saga snapshot repository.
    /// </summary>
    public ISagaSnapshotRepository? SagaSnapshotRepository { get; set; }

    /// <summary>
    /// Persist saga.
    /// </summary>
    protected virtual async Task PersistAsync()
    {
        if (SagaSnapshotRepository != null)
            await SagaSnapshotRepository.PersistSagaSnapshotAsync(this);
    }
}