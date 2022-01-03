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
    private readonly ICommandResultEvaluator<string?, string?> _resultEvaluator;

    public FakeSaga(Dictionary<int, SagaStep> steps, ISagaCommandDispatcher commandDispatcher,
        ICommandResultEvaluator<string?, string?> resultEvaluator, int cancelOnStep = 0,
        CancellationTokenSource? tokenSource = null)
        : base(new SagaConfigurationOptions())
    {
        _commandDispatcher = commandDispatcher;
        _resultEvaluator = resultEvaluator;
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
        => await ProcessCommandResultAsync(Steps[CurrentStep], compensating);

    public async Task ProcessCommandResultAsync(Customer commandResult, bool compensating)
        => await ProcessCommandResultAsync(Steps[CurrentStep], compensating);

    public async Task ProcessCommandResultAsync(Inventory commandResult, bool compensating)
        => await ProcessCommandResultAsync(Steps[CurrentStep], compensating);

    private async Task ProcessCommandResultAsync(SagaStep step, bool compensating)
    {
        var commandSuccessful = await _resultEvaluator.EvaluateStepResultAsync(
            Steps[CurrentStep], compensating, CancellationToken);
        StateInfo = _resultEvaluator.SagaStateInfo;
        await TransitionSagaStateAsync(commandSuccessful);
    }
}