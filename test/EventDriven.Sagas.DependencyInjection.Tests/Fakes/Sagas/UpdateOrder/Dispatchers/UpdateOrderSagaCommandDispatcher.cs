using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Dispatchers;

public class UpdateOrderSagaCommandDispatcher : SagaCommandDispatcher
{
    public UpdateOrderSagaCommandDispatcher(IEnumerable<ISagaCommandHandler> sagaCommandHandlers) : base(sagaCommandHandlers)
    {
    }

    public override Task DispatchCommandAsync(SagaCommand command, bool compensating)
    {
        throw new NotImplementedException();
    }
}