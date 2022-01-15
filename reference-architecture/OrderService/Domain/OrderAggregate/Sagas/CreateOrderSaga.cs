using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
using EventDriven.Sagas.Persistence.Abstractions;
using OrderService.Domain.OrderAggregate.Sagas.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas;

public class CreateOrderSaga :
    PersistableSaga,
    ISagaCommandResultHandler<OrderState>
{
    public CreateOrderSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator> commandResultEvaluators) :
        base(sagaCommandDispatcher, commandResultEvaluators)
    {
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = GetCurrentAction();
        if (action.Command is CreateOrder && Entity is Order order)
        {
            SetActionStateStarted(action);
            SetActionCommand(action, order);
            await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
            return;
        }
        await base.ExecuteCurrentActionAsync();
    }

    public async Task HandleCommandResultAsync(OrderState result, bool compensating)
    {
        SetCurrentActionCommandResult(result);
        await HandleCommandResultForStepAsync<CreateOrderSaga, OrderState, OrderState>(compensating);
    }

    protected override async Task<bool> CheckLock(Guid entityId)
    {
        if (CheckLockCommandHandler is CheckSagaLockCommandHandler<CreateOrderSaga> handler)
            return await handler.HandleCommandAsync(new CheckSagaLockCommand(entityId));
        return false;
    }
}