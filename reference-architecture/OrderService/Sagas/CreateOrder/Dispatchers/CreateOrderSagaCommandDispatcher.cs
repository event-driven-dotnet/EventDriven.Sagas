using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Sagas.CreateOrder.Commands;

namespace OrderService.Sagas.CreateOrder.Dispatchers;

public class CreateOrderSagaCommandDispatcher : SagaCommandDispatcher
{
    public CreateOrderSagaCommandDispatcher(IEnumerable<ISagaCommandHandler> sagaCommandHandlers) :
        base(sagaCommandHandlers)
    {
    }

    public override async Task DispatchCommandAsync(SagaCommand command, bool compensating)
    {
        switch (command.GetType().Name)
        {
            case nameof(Commands.CreateOrder):
                await DispatchSagaCommandHandlerAsync<Commands.CreateOrder>(command);
                break;
            case nameof(SetOrderStateInitial):
                await DispatchSagaCommandHandlerAsync<SetOrderStateInitial>(command);
                break;
            case nameof(ReserveCustomerCredit):
                await DispatchSagaCommandHandlerAsync<ReserveCustomerCredit>(command);
                break;
            case nameof(ReleaseCustomerCredit):
                await DispatchSagaCommandHandlerAsync<ReleaseCustomerCredit>(command);
                break;
            case nameof(ReserveProductInventory):
                await DispatchSagaCommandHandlerAsync<ReserveProductInventory>(command);
                break;
            case nameof(ReleaseProductInventory):
                await DispatchSagaCommandHandlerAsync<ReleaseProductInventory>(command);
                break;
            case nameof(SetOrderStateCreated):
                await DispatchSagaCommandHandlerAsync<SetOrderStateCreated>(command);
                break;
        }
    }
}