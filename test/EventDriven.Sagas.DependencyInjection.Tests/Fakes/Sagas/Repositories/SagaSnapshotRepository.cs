using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.Repositories;

public class SagaSnapshotRepository : ISagaSnapshotRepository
{
    public Task RetrieveAsync(Guid id, PersistableSaga entity)
    {
        throw new NotImplementedException();
    }

    public Task PersistAsync(PersistableSaga entity)
    {
        throw new NotImplementedException();
    }
}