using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Handlers;

public class UpdateOrderCommandHandler :
    ResultDispatchingSagaCommandHandler<UpdateOrderSaga, Commands.UpdateOrder, OrderState>
{
    public override Task HandleCommandAsync(Commands.UpdateOrder command)
    {
        throw new NotImplementedException();
    }
}