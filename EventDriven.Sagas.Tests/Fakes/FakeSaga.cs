using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;

namespace EventDriven.Sagas.Tests.Fakes;

public class FakeSaga : Saga,
    ICommandResultProcessor<Order>,
    ICommandResultProcessor<Customer>,
    ICommandResultProcessor<Inventory>
{
    private readonly ISagaCommandDispatcher _commandDispatcher;

    public FakeSaga(Dictionary<int, SagaStep> steps, ISagaCommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
        Steps = steps;
    }

    public override async Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        // Set state and current step
        State = SagaState.Executing;
        CurrentStep = 1;

        // Dispatch current step command
        await ExecuteCurrentActionAsync();
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        await _commandDispatcher.DispatchAsync(Steps[CurrentStep].Action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        await _commandDispatcher.DispatchAsync(Steps[CurrentStep].CompensatingAction.Command, true);
    }

    public async Task ProcessCommandResultAsync(Order commandResult, bool compensating)
    {
        // Evaluate command result
        var actionCommand = compensating
            ? (FakeCommand)Steps[CurrentStep].CompensatingAction.Command
            : (FakeCommand)Steps[CurrentStep].Action.Command;
        var commandSuccessful = string.Compare(commandResult.State, actionCommand.ExpectedResult, StringComparison.OrdinalIgnoreCase) == 0;
        
        // Transition saga state
        await TransitionSagaStateAsync(commandSuccessful);    
    }

    public async Task ProcessCommandResultAsync(Customer commandResult, bool compensating)
    {
        // Evaluate command result
        var actionCommand = compensating
            ? (FakeCommand) Steps[CurrentStep].CompensatingAction.Command
            : (FakeCommand) Steps[CurrentStep].Action.Command;
        var commandSuccessful = string.Compare(commandResult.Credit, actionCommand.ExpectedResult, StringComparison.OrdinalIgnoreCase) == 0;

        // Transition saga state
        await TransitionSagaStateAsync(commandSuccessful);
    }

    public async Task ProcessCommandResultAsync(Inventory commandResult, bool compensating)
    {
        // Evaluate command result
        var actionCommand = compensating
            ? (FakeCommand)Steps[CurrentStep].CompensatingAction.Command
            : (FakeCommand)Steps[CurrentStep].Action.Command;
        var commandSuccessful = string.Compare(commandResult.Stock, actionCommand.ExpectedResult, StringComparison.OrdinalIgnoreCase) == 0;

        // Transition saga state
        await TransitionSagaStateAsync(commandSuccessful);
    }
}