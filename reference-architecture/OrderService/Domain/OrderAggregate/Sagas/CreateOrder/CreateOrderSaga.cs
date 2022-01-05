using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.CreateOrder;

public class CreateOrderSaga : SagaConfig,
    ICommandResultProcessor<Order>
{
    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = Steps[CurrentStep].Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        if (SagaCommandDispatcher != null)
            await SagaCommandDispatcher.DispatchAsync(action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps[CurrentStep].CompensatingAction;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        if (SagaCommandDispatcher != null)
            await SagaCommandDispatcher.DispatchAsync(action.Command, true);
    }

    public async Task ProcessCommandResultAsync(Order commandResult, bool compensating)
        => await ProcessCommandResultAsync(Steps[CurrentStep], compensating);

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