using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Persistence.Abstractions;
using Integration.Models;
using OrderService.Domain.OrderAggregate;
using OrderService.Sagas.Commands;

namespace OrderService.Sagas;

public class CreateOrderSaga :
    PersistableSaga,
    ISagaCommandResultHandler<OrderState>,
    ISagaCommandResultHandler<CustomerCreditReserveResponse>,
    ISagaCommandResultHandler<CustomerCreditReleaseResponse>
{
    public CreateOrderSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) :
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }

    protected override async Task<bool> CheckLock(Guid entityId)
    {
        if (CheckLockCommandHandler is CheckSagaLockCommandHandler<CreateOrderSaga> handler)
            return await handler.HandleCommandAsync(new CheckSagaLockCommand(entityId));
        return false;
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = GetCurrentAction();
        if (Entity is Order order)
        {
            switch (action.Command)
            {
                case CreateOrder:
                    SetActionStateStarted(action);
                    SetActionCommand(action, order);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
                case ReserveCustomerCredit command:
                    command.CustomerId = order.CustomerId;
                    command.CreditRequested = order.OrderItems.Sum(e => e.ProductPrice);
                    SetActionStateStarted(action);
                    SetActionCommand(action);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
                case SetOrderStateCreated:
                    SetActionStateStarted(action);
                    SetActionCommand(action, order);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
            }
            await PersistAsync();
            return;
        }
        await base.ExecuteCurrentActionAsync();
    }

    public async Task HandleCommandResultAsync(OrderState result, bool compensating)
    {
        SetCurrentActionCommandResult(result);
        await HandleCommandResultForStepAsync<CreateOrderSaga, OrderState, OrderState>(compensating);
    }

    public async Task HandleCommandResultAsync(CustomerCreditReserveResponse result, bool compensating)
    {
        SetCurrentActionCommandResult(result);
        await HandleCommandResultForStepAsync<CreateOrderSaga, CustomerCreditReserveResponse,
            CustomerCreditReserveResponse>(compensating);
    }

    public async Task HandleCommandResultAsync(CustomerCreditReleaseResponse result, bool compensating)
    {
        SetCurrentActionCommandResult(result);
        await HandleCommandResultForStepAsync<CreateOrderSaga, CustomerCreditReleaseResponse,
            CustomerCreditReleaseResponse>(compensating);
    }
}