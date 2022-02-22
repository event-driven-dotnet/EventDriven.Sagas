using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;
using Xunit;

namespace EventDriven.Sagas.Abstractions.Tests.Saga;

public class SagaTests
{
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async Task SagaShouldCompleteToStep(int step)
    {
        // Arrange
        var dispatcher = new InMemoryCommandDispatcher();
        var configRepo = new FakeSagaConfigRepository();
        var snapshotRepo = new FakeSnapshotRepository();
        var resultEvaluators = new List<FakeCommandResultEvaluator>{ new() };
        var config = await configRepo.GetAsync(Guid.Empty);
        var steps = new List<SagaStep>(config?.Steps.Where(s => s.Sequence <= step)
                                       ?? Array.Empty<SagaStep>());
        var saga = new FakeSaga(steps, dispatcher, resultEvaluators, snapshotRepo);
        var order = new Order();
        var customer = new Customer();
        var inventory = new Inventory();
        dispatcher.OrderCommandHandler = new OrderCommandHandler(order, saga);
        dispatcher.CustomerCommandHandler = new CustomerCommandHandler(customer, saga);
        dispatcher.InventoryCommandHandler = new InventoryCommandHandler(inventory, saga);

        // Act
        await saga.StartSagaAsync();

        // Assert
        var expectedOrderState = "Initial";
        var expectedCustomerCredit = "Available";
        var expectedInventoryStock = "Available";
        var expectedActionStates = Enumerable.Empty<ActionState>();
        var actionStates = steps.Select(s => s.Action.State);
        var compensatingActionStates = steps.Select(s => s.CompensatingAction.State);
        var expectedCompensatingActionStates = Enumerable.Empty<ActionState>();

        switch (step)
        {
            case 1:
                expectedOrderState = "Pending";
                expectedActionStates = Enumerable.Repeat(ActionState.Succeeded, 1);
                expectedCompensatingActionStates = Enumerable.Repeat(ActionState.Initial, 1);
                break;
            case 2:
                expectedOrderState = "Pending";
                expectedCustomerCredit = "Reserved";
                expectedActionStates = Enumerable.Repeat(ActionState.Succeeded, 2);
                expectedCompensatingActionStates = Enumerable.Repeat(ActionState.Initial, 2);
                break;
            case 3:
                expectedOrderState = "Pending";
                expectedCustomerCredit = "Reserved";
                expectedInventoryStock = "Reserved";
                expectedActionStates = Enumerable.Repeat(ActionState.Succeeded, 3);
                expectedCompensatingActionStates = Enumerable.Repeat(ActionState.Initial, 3);
                break;
            case 4:
                expectedOrderState = "Created";
                expectedCustomerCredit = "Reserved";
                expectedInventoryStock = "Reserved";
                expectedActionStates = Enumerable.Repeat(ActionState.Succeeded, 4);
                expectedCompensatingActionStates = Enumerable.Repeat(ActionState.Initial, 4);
                break;
        }

        Assert.Equal(expectedOrderState, order.State);
        Assert.Equal(expectedCustomerCredit, customer.Credit);
        Assert.Equal(expectedInventoryStock, inventory.Stock);
        Assert.Equal(SagaState.Executed, saga.State);
        Assert.Equal(expectedActionStates, actionStates);
        Assert.Equal(expectedCompensatingActionStates, compensatingActionStates);
        Assert.Equal(step, snapshotRepo.Sagas.Count);
    }

    [Theory]
    [InlineData(1, false)]
    [InlineData(1, true)]
    [InlineData(2, false)]
    [InlineData(3, false)]
    [InlineData(4, false)]
    public async Task SagaShouldFailOnStep(int step, bool cancel)
    {
        // Arrange
        var tokenSource = new CancellationTokenSource();
        var dispatcher = new InMemoryCommandDispatcher();
        var configRepo = new FakeSagaConfigRepository();
        var snapshotRepo = new FakeSnapshotRepository();
        var resultEvaluators = new List<FakeCommandResultEvaluator>{ new() };
        var config = await configRepo.GetAsync(Guid.Empty);
        var steps = new List<SagaStep>(config?.Steps.Where(s => s.Sequence <= step)
            ?? Array.Empty<SagaStep>());
        var cancelOnStep = cancel ? step : 0;
        var saga = new FakeSaga(steps, dispatcher, resultEvaluators, snapshotRepo, cancelOnStep, tokenSource);
        var order = new Order();
        var customer = new Customer();
        var inventory = new Inventory();
        dispatcher.OrderCommandHandler = new OrderCommandHandler(order, saga);
        dispatcher.CustomerCommandHandler = new CustomerCommandHandler(customer, saga);
        dispatcher.InventoryCommandHandler = new InventoryCommandHandler(inventory, saga);
        var cmd = (FakeCommand)steps.Single(s => s.Sequence == step).Action.Command;
        cmd.Result = "Foo";

        // Act
        await saga.StartSagaAsync(cancellationToken: tokenSource.Token);

        // Assert
        string? expectedStateInfo;
        string? expectedSagaState = null;
        var expectedOrderState = "Initial";
        var expectedCustomerCredit = "Available";
        var expectedInventoryStock = "Available";
        var expectedActionStates = Enumerable.Empty<ActionState>();
        var actionStates = steps.Select(s => s.Action.State);
        var expectedStateInfos = Enumerable.Empty<string?>();
        var stateInfos = steps.Select(s => s.Action.StateInfo);
        string cancelMessage = "Cancellation requested.";

        switch (step)
        {
            case 1:
                expectedActionStates = new List<ActionState>
                    { cancel ? ActionState.Cancelled : ActionState.Failed };
                expectedStateInfo = cancel ? cancelMessage : "'Foo' returned when 'Pending' was expected.";
                expectedStateInfos = new List<string?>
                    { expectedStateInfo };
                expectedSagaState = $"Step 1 command 'SetStatePending' failed. {expectedStateInfo}";
                break;
            case 2:
                expectedActionStates = new List<ActionState>
                    { ActionState.Succeeded, cancel ? ActionState.Cancelled : ActionState.Failed };
                expectedStateInfo = cancel ? cancelMessage : "'Foo' returned when 'Reserved' was expected.";
                expectedStateInfos = new List<string?>
                    { null, expectedStateInfo };
                expectedSagaState = $"Step 2 command 'ReserveCredit' failed. {expectedStateInfo}";
                break;
            case 3:
                expectedActionStates = new List<ActionState>
                    { ActionState.Succeeded, ActionState.Succeeded, cancel ? ActionState.Cancelled : ActionState.Failed };
                expectedStateInfo = cancel ? cancelMessage : "'Foo' returned when 'Reserved' was expected.";
                expectedStateInfos = new List<string?>
                    { null, null, expectedStateInfo };
                expectedSagaState = $"Step 3 command 'ReserveInventory' failed. {expectedStateInfo}";
                break;
            case 4:
                expectedActionStates = new List<ActionState>
                    { ActionState.Succeeded, ActionState.Succeeded, ActionState.Succeeded, cancel ? ActionState.Cancelled : ActionState.Failed };
                expectedStateInfo = cancel ? cancelMessage : "'Foo' returned when 'Created' was expected.";
                expectedStateInfos = new List<string?>
                    { null, null, null, expectedStateInfo };
                expectedSagaState = $"Step 4 command 'SetStateCreated' failed. {expectedStateInfo}";
                break;
        }

        Assert.Equal(expectedOrderState, order.State);
        Assert.Equal(expectedCustomerCredit, customer.Credit);
        Assert.Equal(expectedInventoryStock, inventory.Stock);
        Assert.Equal(SagaState.Compensated, saga.State);
        Assert.Equal(expectedActionStates, actionStates);
        Assert.Equal(expectedStateInfos, stateInfos);
        Assert.Equal(expectedSagaState, saga.StateInfo);
        Assert.Equal(step * 2, snapshotRepo.Sagas.Count);
    }
}