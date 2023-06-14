using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;

namespace EventDriven.Sagas.Persistence.Abstractions.Tests.Fakes;

public class FakeCommandDispatcher : ISagaCommandDispatcher
{
    public Task DispatchCommandAsync(SagaCommand command, bool compensating) => throw new NotImplementedException();
}