using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Persistence.Abstractions;

namespace OrderService.Domain.OrderAggregate.Sagas;

public class CreateOrderSaga :
    PersistableSaga,
    ISagaCommandResultHandler<OrderState>
{
    public CreateOrderSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        ISagaCommandResultEvaluator commandResultEvaluator) :
        base(sagaCommandDispatcher, commandResultEvaluator)
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
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        if (action.Command is SagaCommand<OrderState, OrderState> command)
            command.Result = result;
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        await HandleCommandResultAsync(step, compensating);
    }

    private async Task HandleCommandResultAsync(SagaStep step, bool compensating)
    {
        var commandSuccessful = await CommandResultEvaluator.EvaluateStepResultAsync(
            step, compensating, CancellationToken);
        StateInfo = CommandResultEvaluator.SagaStateInfo;
        await TransitionSagaStateAsync(commandSuccessful);
    }
}