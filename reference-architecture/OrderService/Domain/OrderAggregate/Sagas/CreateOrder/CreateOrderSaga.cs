using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands;

namespace OrderService.Domain.OrderAggregate.Sagas.CreateOrder;

public class CreateOrderSaga : Saga,
    ICommandResultProcessor<Order>
{
    private readonly ISagaCommandDispatcher _commandDispatcher;

    public CreateOrderSaga(ISagaCommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = Steps[CurrentStep].Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        await _commandDispatcher.DispatchAsync(action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps[CurrentStep].CompensatingAction;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        await _commandDispatcher.DispatchAsync(action.Command, true);
    }

    public async Task ProcessCommandResultAsync(Order commandResult, bool compensating)
    {
        // Evaluate result
        var isCancelled = !compensating && CancellationToken.IsCancellationRequested;
        var action = compensating ? Steps[CurrentStep].CompensatingAction : Steps[CurrentStep].Action;
        var expectedResult = ((SetOrderStatePending)action.Command).ExpectedResult;
        var commandSuccessful = !isCancelled && commandResult.State == expectedResult;

        // Check timeout
        action.Completed = DateTime.UtcNow;
        action.Duration = action.Completed - action.Started;
        var commandTimedOut = commandSuccessful && action.Timeout != null && action.Duration > action.Timeout;
        if (commandTimedOut) commandSuccessful = false;

        // Transition action state
        action.State = ActionState.Succeeded;
        if (!commandSuccessful)
        {
            if (isCancelled)
            {
                action.State = ActionState.Cancelled;
                action.StateInfo = "Cancellation requested.";
            }
            else if (!commandTimedOut)
            {
                action.State = ActionState.Failed;
                action.StateInfo = $"Unexpected result: '{commandResult.State}'.";
            }
            else
            {
                action.State = ActionState.TimedOut;
                action.StateInfo =
                    $"Duration of '{action.Duration!.Value:c}' exceeded timeout of '{action.Timeout!.Value:c}'";
            }

            var commandName = action.Command.Name ?? "No name";
            StateInfo = $"Step {CurrentStep} command '{commandName}' failed. {action.StateInfo}";
        }

        // Transition saga state
        await TransitionSagaStateAsync(commandSuccessful);
    }
}