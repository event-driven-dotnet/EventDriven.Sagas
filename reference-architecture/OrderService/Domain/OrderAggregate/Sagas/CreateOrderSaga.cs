using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Persistence.Abstractions;

namespace OrderService.Domain.OrderAggregate.Sagas;

public class CreateOrderSaga : PersistableSaga<Order>
{
    public CreateOrderSaga(
        ISagaCommandDispatcher sagaCommandDispatcher,
        ICommandResultEvaluator commandResultEvaluator) :
        base(sagaCommandDispatcher, commandResultEvaluator)
    {
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        action.Command = action.Command with { EntityId = Entity.Id };
        await PersistAsync();
        await SagaCommandDispatcher.DispatchAsync(action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        action.Command = action.Command with { EntityId = Entity.Id };
        await PersistAsync();
        await SagaCommandDispatcher.DispatchAsync(action.Command, true);
    }

    public override async Task ProcessCommandResultAsync(Order commandResult, bool compensating)
    {
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        await ProcessCommandResultAsync(step, compensating);
    }

    private async Task ProcessCommandResultAsync(SagaStep step, bool compensating)
    {
        var commandSuccessful = await CommandResultEvaluator.EvaluateStepResultAsync(
            step, compensating, CancellationToken);
        StateInfo = CommandResultEvaluator.SagaStateInfo;
        await TransitionSagaStateAsync(commandSuccessful);
    }
}