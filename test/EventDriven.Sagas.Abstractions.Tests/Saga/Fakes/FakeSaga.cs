using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Dispatchers;
using EventDriven.Sagas.Abstractions.Evaluators;
using EventDriven.Sagas.Abstractions.Handlers;
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

    protected override Task<bool> CheckLock(Guid entityId) => Task.FromResult(false);

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
        await HandleCommandResultForStepAsync<FakeSaga, string, string>(compensating);
    }

    public async Task HandleCommandResultAsync(Customer result, bool compensating)
    {
        await HandleCommandResultForStepAsync<FakeSaga, string, string>(compensating);
    }

    public async Task HandleCommandResultAsync(Inventory result, bool compensating)
    {
        await HandleCommandResultForStepAsync<FakeSaga, string, string>(compensating);
    }
}