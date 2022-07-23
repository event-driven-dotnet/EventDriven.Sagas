using System.Diagnostics;
using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Persistence.Abstractions;
using OrderService.Domain.OrderAggregate;
using OrderService.Sagas.CreateOrder.Commands;

namespace OrderService.Sagas.CreateOrder;

public class CreateOrderSaga :
    PersistableSaga<OrderMetadata>,
    ISagaCommandResultHandler<OrderState>,
    ISagaCommandResultHandler<CustomerCreditReserveResponse>,
    ISagaCommandResultHandler<CustomerCreditReleaseResponse>,
    ISagaCommandResultHandler<ProductInventoryReserveResponse>,
    ISagaCommandResultHandler<ProductInventoryReleaseResponse>
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
        // Retrieve order metadata
        Debug.Print("Order metadata: {0}: {1}, {2}, {3}",
            Metadata?.VendorInfo.Name, Metadata?.VendorInfo.City, Metadata?.VendorInfo.State, Metadata?.VendorInfo.Country);
        
        var action = GetCurrentAction();
        if (Entity is Order order)
        {
            switch (action.Command)
            {
                case Commands.CreateOrder:
                    SetActionStateStarted(action);
                    SetActionCommand(action, order);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
                case ReserveCustomerCredit command:
                    command.CustomerId = order.CustomerId;
                    command.CreditRequested = order.Quantity * order.OrderItems.Sum(e => e.ProductPrice);
                    SetActionStateStarted(action);
                    SetActionCommand(action);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
                case ReserveProductInventory command:
                    command.InventoryId = order.InventoryId;
                    command.AmountRequested = order.Quantity;
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
            return;
        }
        await base.ExecuteCurrentActionAsync();
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = GetCurrentCompensatingAction();
        if (Entity is Order order)
        {
            switch (action.Command)
            {
                case ReleaseProductInventory command:
                    command.InventoryId = order.InventoryId;
                    command.AmountRequested = order.Quantity;
                    SetActionStateStarted(action);
                    SetActionCommand(action);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, true);
                    break;
                case ReleaseCustomerCredit command:
                    command.CustomerId = order.CustomerId;
                    command.CreditReleased = order.Quantity * order.OrderItems.Sum(e => e.ProductPrice);
                    SetActionStateStarted(action);
                    SetActionCommand(action);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, true);
                    break;
                case SetOrderStateInitial:
                    SetActionStateStarted(action);
                    SetActionCommand(action);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, true);
                    break;
            }
            return;
        }
        await base.ExecuteCurrentCompensatingActionAsync();
    }

    protected override async Task ExecuteAfterStep() => await PersistAsync();

    public async Task HandleCommandResultAsync(OrderState result, bool compensating)
    {
        SetCurrentActionCommandResult(result, compensating);
        await HandleCommandResultForStepAsync<CreateOrderSaga, OrderState, OrderState>(compensating);
    }

    public async Task HandleCommandResultAsync(CustomerCreditReserveResponse result, bool compensating)
    {
        SetCurrentActionCommandResult(result, compensating);
        await HandleCommandResultForStepAsync<CreateOrderSaga, CustomerCreditReserveResponse,
            CustomerCreditReserveResponse>(compensating);
    }

    public async Task HandleCommandResultAsync(CustomerCreditReleaseResponse result, bool compensating)
    {
        SetCurrentActionCommandResult(result, compensating);
        await HandleCommandResultForStepAsync<CreateOrderSaga, CustomerCreditReleaseResponse,
            CustomerCreditReleaseResponse>(compensating);
    }
    
    public async Task HandleCommandResultAsync(ProductInventoryReserveResponse result, bool compensating)
    {
        SetCurrentActionCommandResult(result, compensating);
        await HandleCommandResultForStepAsync<CreateOrderSaga, ProductInventoryReserveResponse,
            ProductInventoryReserveResponse>(compensating);
    }
    
    public async Task HandleCommandResultAsync(ProductInventoryReleaseResponse result, bool compensating)
    {
        SetCurrentActionCommandResult(result, compensating);
        await HandleCommandResultForStepAsync<CreateOrderSaga, ProductInventoryReleaseResponse,
            ProductInventoryReleaseResponse>(compensating);
    }
}