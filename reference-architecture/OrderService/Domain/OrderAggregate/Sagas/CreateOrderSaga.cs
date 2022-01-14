using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Persistence.Abstractions;

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
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        action.Command = action.Command with { EntityId = EntityId };
        await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
        await PersistAsync();
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        action.Command = action.Command with { EntityId = EntityId };
        await SagaCommandDispatcher.DispatchCommandAsync(action.Command, true);
        await PersistAsync();
    }
    
    public async Task HandleCommandResultAsync(OrderState result, bool compensating)
    {
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        var action = step.Action;
        if (action.Command is SagaCommand<OrderState, OrderState> command)
            command.Result = result;
        await HandleCommandResultForStepAsync<CreateOrderSaga, OrderState, OrderState>(step, compensating);
    }
}