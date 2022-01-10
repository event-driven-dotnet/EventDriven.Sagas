using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeSnapshotRepository : ISagaSnapshotRepository
{
    public List<PersistableSaga> Sagas { get; } = new();

    public Task RetrieveSagaSnapshotAsync(Guid id, PersistableSaga entity)
    {
        var result = Sagas.LastOrDefault(e => e.Id == id);
        return Task.FromResult(result);
    }

    public Task PersistSagaSnapshotAsync(PersistableSaga entity)
    {
        Sagas.Add(entity);
        return Task.FromResult(entity);
    }
}