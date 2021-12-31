using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Tests.Fakes;
using Xunit;

namespace EventDriven.Sagas.Tests
{
    public class SagaTests
    {
        private readonly Dictionary<int, SagaStep> _steps;

        public SagaTests()
        {
            _steps = new Dictionary<int, SagaStep>
            {
                {   1,
                    new SagaStep
                    {
                        Sequence = 1,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStatePending",
                                Payload = "Pending",
                                ExpectedResult = "Pending"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateInitial",
                                Payload = "Initial",
                                ExpectedResult = "Initial"
                            }
                        }
                    }
                },
                {   2,
                    new SagaStep
                    {
                        Sequence = 2,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReserveCredit",
                                Payload = "Reserved",
                                ExpectedResult = "Reserved"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReleaseCredit",
                                Payload = "Available",
                                ExpectedResult = "Available"
                            }
                        }
                    }
                },
                {   3,
                    new SagaStep
                    {
                        Sequence = 3,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReserveInventory",
                                Payload = "Reserved",
                                ExpectedResult = "Reserved"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "ReleaseInventory",
                                Payload = "Available",
                                ExpectedResult = "Available"
                            }
                        }
                    }
                },
                {   4,
                    new SagaStep
                    {
                        Sequence = 4,
                        Action = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateCreated",
                                Payload = "Created",
                                ExpectedResult = "Created"
                            }
                        },
                        CompensatingAction = new SagaAction
                        {
                            Command = new FakeCommand
                            {
                                Name = "SetStateInitial",
                                Payload = "Initial",
                                ExpectedResult = "Initial"
                            }
                        }
                    }
                },
            };
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task SagaShouldCompleteToStep(int step)
        {
            // Arrange
            var dispatcher = new InMemoryCommandDispatcher();
            var steps = new Dictionary<int, SagaStep>(_steps.Where(s => s.Key <= step));
            var saga = new FakeSaga(steps, dispatcher);
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
            var actionStates = steps.Select(s => s.Value.Action.State);
            var compensatingActionStates = steps.Select(s => s.Value.CompensatingAction.State);
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
            var steps = new Dictionary<int, SagaStep>(_steps.Where(s => s.Key <= step));
            var cancelOnStep = cancel ? step : 0;
            var saga = new FakeSaga(steps, dispatcher, cancelOnStep, tokenSource);
            var order = new Order();
            var customer = new Customer();
            var inventory = new Inventory();
            dispatcher.OrderCommandHandler = new OrderCommandHandler(order, saga);
            dispatcher.CustomerCommandHandler = new CustomerCommandHandler(customer, saga);
            dispatcher.InventoryCommandHandler = new InventoryCommandHandler(inventory, saga);
            ((FakeCommand)steps[step].Action.Command).Payload = "Foo";

            // Act
            await saga.StartSagaAsync(tokenSource.Token);

            // Assert
            var expectedOrderState = "Initial";
            var expectedCustomerCredit = "Available";
            var expectedInventoryStock = "Available";
            var expectedActionStates = Enumerable.Empty<ActionState>();
            var actionStates = steps.Select(s => s.Value.Action.State);
            var expectedStateInfos = Enumerable.Empty<string?>();
            var stateInfos = steps.Select(s => s.Value.Action.StateInfo);
            string? expectedSagaState = null;
            var stateMessage = cancel ? "Cancellation requested." : "Unexpected result: 'Foo'.";

            switch (step)
            {
                case 1:
                    expectedActionStates = new List<ActionState>
                        { cancel ? ActionState.Cancelled : ActionState.Failed };
                    expectedStateInfos = new List<string?>
                        { stateMessage };
                    expectedSagaState = $"Step 1 command 'SetStatePending' failed. {stateMessage}";
                    break;
                case 2:
                    expectedActionStates = new List<ActionState>
                        { ActionState.Succeeded, cancel ? ActionState.Cancelled : ActionState.Failed };
                    expectedStateInfos = new List<string?>
                        { null, stateMessage };
                    expectedSagaState = $"Step 2 command 'ReserveCredit' failed. {stateMessage}";
                    break;
                case 3:
                    expectedActionStates = new List<ActionState>
                        { ActionState.Succeeded, ActionState.Succeeded, cancel ? ActionState.Cancelled : ActionState.Failed };
                    expectedStateInfos = new List<string?>
                        { null, null, stateMessage };
                    expectedSagaState = $"Step 3 command 'ReserveInventory' failed. {stateMessage}";
                    break;
                case 4:
                    expectedActionStates = new List<ActionState>
                        { ActionState.Succeeded, ActionState.Succeeded, ActionState.Succeeded, cancel ? ActionState.Cancelled : ActionState.Failed };
                    expectedStateInfos = new List<string?>
                        { null, null, null, stateMessage };
                    expectedSagaState = $"Step 4 command 'SetStateCreated' failed. {stateMessage}";
                    break;
            }

            Assert.Equal(expectedOrderState, order.State);
            Assert.Equal(expectedCustomerCredit, customer.Credit);
            Assert.Equal(expectedInventoryStock, inventory.Stock);
            Assert.Equal(SagaState.Compensated, saga.State);
            Assert.Equal(expectedActionStates, actionStates);
            Assert.Equal(expectedStateInfos, stateInfos);
            Assert.Equal(expectedSagaState, saga.StateInfo);
        }
    }
}