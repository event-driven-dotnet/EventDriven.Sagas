using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Handlers;

public class CreateOrderCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, Commands.CreateOrder, OrderState>
{
    public override Task HandleCommandAsync(Commands.CreateOrder command)
    {
        throw new NotImplementedException();
    }
}