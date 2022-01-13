using EventDriven.Sagas.Abstractions.Commands;
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
        ISagaCommandResultEvaluator commandResultEvaluator) : 
        base(sagaCommandDispatcher, commandResultEvaluator)
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