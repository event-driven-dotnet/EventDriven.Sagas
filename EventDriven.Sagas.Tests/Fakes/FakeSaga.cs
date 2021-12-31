using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeSaga : Saga,
    ICommandResultProcessor<Order>,
    ICommandResultProcessor<Customer>,
    ICommandResultProcessor<Inventory>
{
    private readonly int _cancelOnStep;
    private readonly CancellationTokenSource? _tokenSource;
    private readonly ISagaCommandDispatcher _commandDispatcher;

    public FakeSaga(Dictionary<int, SagaStep> steps, ISagaCommandDispatcher commandDispatcher,
        int cancelOnStep = 0, CancellationTokenSource? tokenSource = null)
    {
        _commandDispatcher = commandDispatcher;
        _cancelOnStep = cancelOnStep;
        _tokenSource = tokenSource;
        Steps = steps;
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        if (CurrentStep == _cancelOnStep && _tokenSource != null) _tokenSource.Cancel();
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
        await ProcessCommandResultLocalAsync(commandResult, compensating);    
    }

    public async Task ProcessCommandResultAsync(Customer commandResult, bool compensating)
    {
        await ProcessCommandResultLocalAsync(commandResult, compensating);
    }

    public async Task ProcessCommandResultAsync(Inventory commandResult, bool compensating)
    {
        await ProcessCommandResultLocalAsync(commandResult, compensating);
    }

    private async Task ProcessCommandResultLocalAsync<TEntity>(TEntity commandResult, bool compensating)
    {
        // Get result
        string? result = null;
        if (commandResult is Order order) result = order.State;
        else if (commandResult is Customer customer) result = customer.Credit;
        else if (commandResult is Inventory inventory) result = inventory.Stock;

        // Evaluate result
        var isCancelled = !compensating && CancellationToken.IsCancellationRequested;
        var action = compensating ? Steps[CurrentStep].CompensatingAction : Steps[CurrentStep].Action;
        var commandSuccessful = !isCancelled && string.Compare(result,
            ((FakeCommand)action.Command).ExpectedResult, StringComparison.OrdinalIgnoreCase) == 0;

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
                action.StateInfo = result != null ? $"Unexpected result: '{result}'." : "Unexpected result.";
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