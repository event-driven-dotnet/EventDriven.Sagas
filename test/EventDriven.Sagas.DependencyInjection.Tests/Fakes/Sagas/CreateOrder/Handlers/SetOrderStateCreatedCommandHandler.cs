using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Domain;
using EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Commands;

namespace EventDriven.Sagas.DependencyInjection.Tests.Fakes.Sagas.CreateOrder.Handlers;

public class SetOrderStateCreatedCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, SetOrderStateCreated, OrderState>
{
    public override Task HandleCommandAsync(SetOrderStateCreated command)
    {
        throw new NotImplementedException();
    }
}