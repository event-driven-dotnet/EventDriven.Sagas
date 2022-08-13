using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Commands;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.UpdateOrder.Handlers;

public class SetOrderStatePendingCommandHandler :
    ResultDispatchingSagaCommandHandler<UpdateOrderSaga, SetOrderStatePending, OrderState>
{
    public override Task HandleCommandAsync(SetOrderStatePending command)
    {
        throw new NotImplementedException();
    }
}