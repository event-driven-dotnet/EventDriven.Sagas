using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;
using EventDriven.Sagas.Persistence.Abstractions;

namespace OrderService.Domain.OrderAggregate.Sagas;

public record CreateOrderSaga : PersistableSaga<Order>, ICommandResultProcessor<Order>
{
    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        action.Command = action.Command with { EntityId = Entity.Id };
        await PersistAsync();
        if (SagaCommandDispatcher != null)
            await SagaCommandDispatcher.DispatchAsync(action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        action.Command = action.Command with { EntityId = Entity.Id };
        await PersistAsync();
        if (SagaCommandDispatcher != null)
            await SagaCommandDispatcher.DispatchAsync(action.Command, true);
    }

    public async Task ProcessCommandResultAsync(Order commandResult, bool compensating)
    {
        await RetrieveAsync();
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        await ProcessCommandResultAsync(step, compensating);
    }

    private async Task ProcessCommandResultAsync(SagaStep step, bool compensating)
    {
        if (CommandResultEvaluator != null)
        {
            var commandSuccessful = await CommandResultEvaluator.EvaluateStepResultAsync(
                step, compensating, CancellationToken);
            StateInfo = CommandResultEvaluator.SagaStateInfo;
            await TransitionSagaStateAsync(commandSuccessful);
        }
    }
}