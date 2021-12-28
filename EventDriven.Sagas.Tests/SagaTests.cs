using System.Collections.Generic;
using System.Linq;
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
            switch (step)
            {
                case 1:
                    expectedOrderState = "Pending";
                    break;
                case 2:
                    expectedOrderState = "Pending";
                    expectedCustomerCredit = "Reserved";
                    break;
                case 3:
                    expectedOrderState = "Pending";
                    expectedCustomerCredit = "Reserved";
                    expectedInventoryStock = "Reserved";
                    break;
                case 4:
                    expectedOrderState = "Created";
                    expectedCustomerCredit = "Reserved";
                    expectedInventoryStock = "Reserved";
                    break;
            }
            Assert.Equal(expectedOrderState, order.State);
            Assert.Equal(expectedCustomerCredit, customer.Credit);
            Assert.Equal(expectedInventoryStock, inventory.Stock);
            Assert.Equal(SagaState.Executed, saga.State);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task SagaShouldFailOnStep(int step)
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
            ((FakeCommand)steps[step].Action.Command).Payload = "Foo";

            // Act
            await saga.StartSagaAsync();

            // Assert
            //Assert.Equal("Initial", order.State);
            var expectedOrderState = "Initial";
            var expectedCustomerCredit = "Available";
            var expectedInventoryStock = "Available";
            Assert.Equal(expectedOrderState, order.State);
            Assert.Equal(expectedCustomerCredit, customer.Credit);
            Assert.Equal(expectedInventoryStock, inventory.Stock);
            Assert.Equal(SagaState.Compensated, saga.State);
        }
    }
}