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
    private readonly SemaphoreSlim _semaphoreSyncRoot;

    /// <inheritdoc />
    protected PersistableSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) : 
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
        _semaphoreSyncRoot = new SemaphoreSlim(1, 1);
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
        try
        {
            await _semaphoreSyncRoot.WaitAsync(LockTimeout, CancellationToken);
            if (SagaSnapshotRepository != null)
                await SagaSnapshotRepository.PersistSagaSnapshotAsync(this);
        }
        finally
        {
            _semaphoreSyncRoot.Release();
        }
    }
}