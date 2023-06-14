using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.Repositories;

public class FakePersistableSagaRepository<TSaga> :
    IPersistableSagaRepository<TSaga>
    where TSaga : PersistableSaga
{
    public Task<TSaga?> GetAsync(Guid id, TSaga newEntity) => throw new NotImplementedException();

    public Task<TSaga> CreateAsync(TSaga newEntity) => throw new NotImplementedException();

    public Task<TSaga> SaveAsync(TSaga existingEntity, TSaga newEntity) => throw new NotImplementedException();

    public Task RemoveAsync(Guid id) => throw new NotImplementedException();
}