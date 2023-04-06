using EventDriven.Sagas.Configuration.Abstractions;
using EventDriven.Sagas.Configuration.Abstractions.Repositories;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.Repositories;

public class FakeSagaConfigRepository : ISagaConfigRepository
{
    public Task<SagaConfiguration?> GetAsync(Guid id) => throw new NotImplementedException();

    public Task<SagaConfiguration?> AddAsync(SagaConfiguration entity) => throw new NotImplementedException();

    public Task<SagaConfiguration?> UpdateAsync(SagaConfiguration entity) => throw new NotImplementedException();

    public Task<int> RemoveAsync(Guid id) => throw new NotImplementedException();
}