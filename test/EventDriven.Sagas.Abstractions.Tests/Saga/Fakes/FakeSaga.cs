using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Persistence.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Repositories;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class FakeSaga : PersistableSaga,
    ISagaCommandResultHandler<Order>,
    ISagaCommandResultHandler<Customer>,
    ISagaCommandResultHandler<Inventory>
{
    private readonly int _cancelOnStep;
    private readonly CancellationTokenSource? _tokenSource;

    public FakeSaga(List<SagaStep> steps, ISagaCommandDispatcher commandDispatcher,
        IEnumerable<ISagaCommandResultEvaluator<string?, string?>> resultEvaluators, 
        ISagaSnapshotRepository sagaSnapshotRepository,
        int cancelOnStep = 0, CancellationTokenSource? tokenSource = null) :
        base(commandDispatcher, resultEvaluators)
    {
        SagaCommandDispatcher = commandDispatcher;
        _cancelOnStep = cancelOnStep;
        _tokenSource = tokenSource;
        Steps = steps;
        SagaSnapshotRepository = sagaSnapshotRepository;
    }

    protected override async Task ExecuteCurrentActionAsync()
    {
        if (CurrentStep == _cancelOnStep && _tokenSource != null) _tokenSource.Cancel();
        var action = Steps.Single(s => s.Sequence == CurrentStep).Action;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        await PersistAsync();
        await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
    }

    protected override async Task ExecuteCurrentCompensatingActionAsync()
    {
        var action = Steps.Single(s => s.Sequence == CurrentStep).CompensatingAction;
        action.State = ActionState.Running;
        action.Started = DateTime.UtcNow;
        await PersistAsync();
        await SagaCommandDispatcher.DispatchCommandAsync(action.Command, true);
    }

    public async Task HandleCommandResultAsync(Order result, bool compensating)
    {
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        await ProcessCommandResultAsync(step, compensating);
    }

    public async Task HandleCommandResultAsync(Customer result, bool compensating)
    {
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        await ProcessCommandResultAsync(step, compensating);
    }

    public async Task HandleCommandResultAsync(Inventory result, bool compensating)
    {
        var step = Steps.Single(s => s.Sequence == CurrentStep);
        await ProcessCommandResultAsync(step, compensating);
    }

    private async Task ProcessCommandResultAsync(SagaStep step, bool compensating)
    {
        await HandleCommandResultForStepAsync<FakeSaga, string, string>(step, compensating);
    }
}