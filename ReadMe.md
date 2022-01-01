# EventDriven.Sagas

Abstractions and reference architecture for implementing the Saga pattern for orchestrating atomic operations which span multiple services.

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (6.0 or greater)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- MongoDB Docker: `docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo`
- [MongoDB Client](https://robomongo.org/download):
  - Download Robo 3T only.
  - Add connection to localhost on port 27017.
- [Dapr](https://dapr.io/) (Distributed Application Runtime)
  - [Install Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
  - [Initialize Dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)

## Packages
- [EventDriven.Sagas.Abstractions](https://www.nuget.org/packages/EventDriven.Sagas.Abstractions)

## Introduction

The purpose of the [Saga pattern](https://microservices.io/patterns/data/saga.html) is to enable **atomic operations which span multiple microservices**, each of which have their own private data store. Each service may perform local transactions against their data store, but distributed transactions in the classical sense are impractical in a miroservices architecture, where holding locks for the two-phase protocol would inhibit performance and scalability.

The Saga pattern consists of a series of steps with actions and corresponding compensating actions. Steps have a sequence number and actions have a command which is executed by a participating microservice and has an expected result. The saga can then use the result as a basis for executing the next step or initiating a rollback with a series of compensating actions. Sagas can be configured and persisted via a durable data store.

The Saga Orchestrator coordinates the entire process by telling each participant what to do and invoking the next participant in either moving the saga forward or rolling it backwards. The orchestrator can run in the same process as the initiating microservice, and it can also communicate with other microservices asynchronously through an event bus abstraction.

<p align="center">
  <img width="900" src="images/saga-orchestration.png">
</p>

### Reference Architecture

#### Run the Create Order Saga

1. Run Dapr Dashboard.
   - Then open http://localhost:8080 to view containers after executing `dapr run` commands.
    ```
    dapr dashboard
    ```
2. Use Dapr to run the customer service.
    ```
    dapr run --app-id customer-service --app-port 5000 --components-path ../dapr/components -- dotnet run
    ```
3. Use Dapr to run the order service.
    ```
    dapr run --app-id order-service --app-port 5150 --components-path ../dapr/components -- dotnet run
    ```

### Development Guide

1. Create a new Web API project.
   ```
   dotnet new webapi --name OrderService
   ```
2. Add NuGet packages.
   - MongoDB.Driver
   - URF.Core.Mongo
   - EventDriven.DDD.Abstractions
   - EventDriven.Sagas.Abstractions
   - AutoMapper.Extensions.Microsoft.DependencyInjection
3. Add a **Domain** folder.
   - Add an **OrderAggregate** folder to the **Domain** folder.
   - Place the OrderState, OrderItem and Order files in this folder.
4. Create an `OrderState` enum.
    ```csharp
    public enum OrderState
    {
        Initial,
        Pending,
        Created
    }
    ```
5. Create an `OrderItem` record.
    ```csharp
    public record OrderItem(Guid ProductId, string ProductName, decimal ProductPrice);
    ```
6. Create an `Order` class.
   - Derive the class from `Entity`.
    ```csharp
    public class Order : Entity
    {
        public Guid CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public List<OrderItem> OrderItems { get; set; } = null!;
        public OrderState State { get; set; }
    }
    ```
7. Add a **Sagas** folder to the **OrderAggregate** folder.
   - Add a **CreateOrder** folder to the **Sagas** folder.
   - Add a `CreateOrderSaga` class to the **CreateOrder** folder.
   - Derive the class from the `Saga` abstract class.
   - Add a ctor that accepts an `ISagaCommandDispatcher`.
   - Flesh out the `ExecuteCurrentActionAsync` method to get the current step's `Action`, set `State` and `Started` properties, then dispatch the action's `Command`.
   - Flesh out the `ExecuteCurrentCompensatingActionAsync` method in the same way for the `CompensatingAction`.
    ```csharp
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

        public Task ProcessCommandResultAsync(Order commandResult, bool compensating)
        {
            throw new NotImplementedException();
        }
    }
    ```
    - Implement `ICommandResultProcessor<Order>` by evaluating the result, checking for timeout and cancellation, then transitioning the saga state.
```csharp
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
```
8. Add a **Commands** folder to the **OrderAggreate** folder.
   - Create a `CreateOrder` record that extends `Command`.
    ```csharp
    public record CreateOrder(Order Order) : Command(Order.Id);
    ```
   - Create a `GetOrderState` record that extends `Command`.
    ```csharp
    public record GetOrderState(Order Order) : Command(Order.Id);
    ```
   - Create a `SetOrderStatePending` record that extends `Command` and implements `ISagaCommand`.
    ```csharp
    public record SetOrderState : Command,
        ISagaCommand<OrderState, OrderState>
    {
        public string? Name { get; set; } = "SetStatePending";
        public OrderState Payload { get; set; }
        public OrderState ExpectedResult { get; set; }
    }
    ```
9. Create a `OrderCommandHandler` class in the **Commands** folder.
   - Implement `ICommandHandler<Order, CreateOrder>`, `ICommandHandler<Order, GetOrderState>` and `ICommandHandler<Order, SetOrderStatePending>`.
   - Add a ctor that accepts a `CreateOrderSaga` and `ICommandResultProcessor<Order>`.
   - Flesh out the `CreateOrder` handler to start a saga to create an order.
   - Flesh out the `SetOrderStatePending` handlers to set the order state to the command payload and call `ProcessCommandResultAsync`.
    ```csharp
    public class OrderCommandHandler :
        ICommandHandler<Order, CreateOrder>,
        ICommandHandler<Order, GetOrderState>,
        ICommandHandler<Order, SetOrderStatePending>
    {
        private Order _order = null!;
        private readonly ILogger<OrderCommandHandler> _logger;
        private readonly CreateOrderSaga _createOrderSaga;

        public OrderCommandHandler(
            ILogger<OrderCommandHandler> logger,
            CreateOrderSaga createOrderSaga)
        {
            _logger = logger;
            _createOrderSaga = createOrderSaga;
        }

        public async Task<CommandResult<Order>> Handle(CreateOrder command)
        {
            _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
            _order = command.Order;

            // Start saga to create an order
            await _createOrderSaga.StartSagaAsync();
            return new CommandResult<Order>(CommandOutcome.Accepted, _order);
        }

        public Task<CommandResult<Order>> Handle(GetOrderState command)
            => Task.FromResult(new CommandResult<Order>(CommandOutcome.Accepted, _order));

        public async Task<CommandResult<Order>> Handle(SetOrderStatePending command)
        {
            _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
            _order.State = command.Payload;
            await _createOrderSaga.ProcessCommandResultAsync(_order, false);
            return new CommandResult<Order>(CommandOutcome.Accepted, _order);
        }
    }
    ```
10. Create a `CreateOrderCommandDispatcher` class that implements `ISagaCommandDispatcher`.
    - Add a ctor that accepts `OrderCommandHandler`.
    - Flesh out the `DispatchAsync` method to dispatch a command to the handler based on the command name.
    ```csharp
    public class CreateOrderCommandDispatcher : ISagaCommandDispatcher
    {
        private readonly OrderCommandHandler _orderCommandHandler;

        public CreateOrderCommandDispatcher(OrderCommandHandler orderCommandHandler)
        {
            _orderCommandHandler = orderCommandHandler;
        }

        public async Task DispatchAsync(ISagaCommand command, bool compensating)
        {
            // Based on command name, dispatch command to handler
            switch (command.Name)
            {
                case "SetStatePending":
                    await _orderCommandHandler.Handle(new SetOrderState
                    {
                        Name = command.Name,
                        Payload = OrderState.Pending
                    });
                    break;
            }
        }
    }
    ```
11. Create a `SagaConfigRepository` class that implements `ISagaConfigRepository`.
    ```csharp
    public class SagaConfigRepository : ISagaConfigRepository
    {
        private readonly IDocumentRepository<SagaConfiguration> _documentRepository;
        private readonly ILogger<SagaConfigRepository> _logger;

        public SagaConfigRepository(
            IDocumentRepository<SagaConfiguration> documentRepository,
            ILogger<SagaConfigRepository> logger)
        {
            _documentRepository = documentRepository;
            _logger = logger;
        }

        public async Task<SagaConfiguration> GetSagaConfigurationAsync(Guid id)
            => await _documentRepository.FindOneAsync(e => e.Id == id);

        public async Task<SagaConfiguration> AddSagaConfigurationAsync(SagaConfiguration entity)
        {
            var existing = await _documentRepository.FindOneAsync(e => e.Id == entity.Id);
            if (existing != null) throw new ConcurrencyException();
            entity.ETag = Guid.NewGuid().ToString();
            return await _documentRepository.InsertOneAsync(entity);
        }

        public async Task<SagaConfiguration> UpdateSagaConfigurationAsync(SagaConfiguration entity)
        {
            var existing = await GetSagaConfigurationAsync(entity.Id);
            if (existing == null || string.Compare(entity.ETag, existing.ETag, StringComparison.OrdinalIgnoreCase) != 0)
                throw new ConcurrencyException();
            entity.ETag = Guid.NewGuid().ToString();
            return await _documentRepository.FindOneAndReplaceAsync(e => e.Id == entity.Id, entity);
        }

        public async Task<int> RemoveSagaConfigurationAsync(Guid id) =>
            await _documentRepository.DeleteOneAsync(e => e.Id == id);
    }
    ```
12. Add a **DTO** folder to the project root, then add a **Write** folder.
    - Add an `Order` class to the **Write** folder.
    - Copy properties from the `Order` entity.
    - Add `Id` and `ETag` from `Entity`.
    - Add an `AutoMapperProfile` class to the **DTO** folder.
    ```csharp
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Order, OrderService.DTO.Write.Order>();
            CreateMap<Order, OrderService.DTO.Write.Order>().ReverseMap();
            CreateMap<OrderItem, OrderService.DTO.Write.OrderItem>();
            CreateMap<OrderItem, OrderService.DTO.Write.OrderItem>().ReverseMap();
            CreateMap<OrderState, OrderService.DTO.Write.OrderState>();
            CreateMap<OrderState, OrderService.DTO.Write.OrderState>().ReverseMap();
        }
    }
    ```
13. Add a `OrderCommandController` class to the **Controllers** folder.
    - Next