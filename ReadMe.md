# EventDriven.Sagas

Abstractions and reference architecture for implementing the Saga pattern to orchestrate atomic operations which span multiple services.

## Prerequisites
- [.NET Core SDK](https://dotnet.microsoft.com/download) (6.0 or greater)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- MongoDB Docker: `docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo`
- [MongoDB Client](https://robomongo.org/download)
  - Download Robo 3T only.
  - Add connection to localhost on port 27017.
- [Dapr](https://dapr.io/) (Distributed Application Runtime)
  - [Install Dapr CLI](https://docs.dapr.io/getting-started/install-dapr-cli/)
  - [Initialize Dapr](https://docs.dapr.io/getting-started/install-dapr-selfhost/)
- [Microsoft Tye](https://github.com/dotnet/tye/blob/main/docs/getting_started.md) (recommended)
- [Specflow](https://specflow.org/) IDE Plugin  (recommended)
  - [Visual Studio](https://docs.specflow.org/projects/getting-started/en/latest/GettingStarted/Step1.html)
  - [JetBrains Rider](https://docs.specflow.org/projects/specflow/en/latest/Rider/rider-installation.html)

## Packages
- [EventDriven.Sagas.Abstractions](https://www.nuget.org/packages/EventDriven.Sagas.Abstractions)
  - Abstractions for sagas, steps, actions, commands, dispatchers, handlers and evaluators.
- [EventDriven.Sagas.Configuration.Abstractions](https://www.nuget.org/packages/EventDriven.Sagas.Configuration.Abstractions)
  - Abstractions for saga configurations.
- [EventDriven.Sagas.Configuration.Mongo](https://www.nuget.org/packages/EventDriven.Sagas.Configuration.Mongo)
  - MongoDB implementation for saga configuration repositories.
- [EventDriven.Sagas.DependencyInjection](https://www.nuget.org/packages/EventDriven.Sagas.DependencyInjection)
  - `AddSaga` service collection extension methods.
- [EventDriven.Sagas.EventBus.Abstractions](https://www.nuget.org/packages/EventDriven.Sagas.EventBus.Abstractions)
  - Abstractions for handling integration events and dispatching command results.
- [EventDriven.Sagas.Persistence.Abstractions](https://www.nuget.org/packages/EventDriven.Sagas.Persistence.Abstractions)
  - Abstractions for persisting saga snapshots.
- [EventDriven.Sagas.Persistence.Mongo](https://www.nuget.org/packages/EventDriven.Sagas.Persistence.Mongo)
  - MongoDB implementation for persisting saga snapshots.

## Tools
- [EventDriven.Sagas.SagaConfig.CLI](https://www.nuget.org/packages/EventDriven.Sagas.SagaConfig.CLI)
  - Tool for generating and posting JSON for a saga configuration definition.
  - ReadMe: [Saga Configuration CLI](administration/SagaConfigCli/ReadMe.md)

## Tests
- **Unit Tests**: EventDriven.Sagas.Abstractions.Tests
- **User Acceptance Tests**: OrderService.Sagas.Specs
  - ReadMe: [Order Service Sagas User Acceptance Tests](test/OrderService.Sagas.Specs/ReadMe.md)

## Reference Architecture
- **SagaConfigDefinition**: Saga Configuration Definition
  - Provides method that returns a `SagaConfigurationDto` containing Create Order Saga steps, actions and commands.
- **Integration**: Models and events that are exchanged between services taking part in the saga.
- **OrderService**: Contains `CreateOrderSaga` with dispatchers, commands, handlers and evaluators for orchestrating a saga with updates that span multiple services.
- **CustomerService**: Contains handlers for processing integration events that update the customer data store when credit is reserved or released.

## Introduction

The purpose of the [Saga pattern](https://microservices.io/patterns/data/saga.html) is to enable **atomic operations which span multiple microservices**, each of which have their own private data store. Each service may perform local transactions against their data store, but distributed transactions in the classical sense are impractical in a miroservices architecture, where holding locks for the two-phase protocol would impair performance and scalability.

The Saga pattern consists of a series of steps with actions and corresponding compensating actions. Steps have a sequence number, and actions have a command which is executed by a participating microservice and has an expected result. The saga can then use the result as a basis for executing the next step or initiating a rollback with a series of compensating actions. Sagas can be configured and persisted via a durable data store.

The Saga orchestrator coordinates the entire process by telling each participant what to do and invoking the next participant in either moving the saga forward or rolling it backwards. The orchestrator can run in the same process as the initiating microservice, and it can also communicate with other microservices asynchronously through an event bus abstraction.

<p align="center">
  <img width="900" src="images/saga-orchestration.png">
</p>

## Running the Sample

### Post saga configuration

1. Run the **SagaConfigService** locally.
   - Open a terminal at the SagaConfigService project.
   - Execute `dotnet run`.
2. Install the **sagaconfig** CLI global tool.
    ```
    dotnet tool install -g EventDriven.Sagas.SagaConfig.CLI --version 1.0.0-beta1
    ```
3. Open a terminal at **reference-architecture/SagaConfigDefinitions**.
   - Run the `sagaconfig` command, passing required parameters.
     - Specify a Guid as the `-id` parameter for the Saga Config Id.
     - Specify a relative path to the location of the **SagaConfigDefinitions.dll** file for the `-p` parameter.
     - Specify the name of the **json** folder for the `-j` parameter.
   - Include the `-uri` parameter to save a config JSON file and post to the SagaConfig Service.
     - First run the SagaConfigService project.
    ```
    sagaconfig -id d89ffb1e-7481-4111-a4dd-ac5123217293 -p bin/Debug/net6.0 -j json -uri http://localhost:5256/api/sagaconfig/
    ```


### Run the services

> **Note**: As an alternative to Tye, you can run services directly usng the Dapr CLI. This may be useful for troubleshooting Dapr issues after setting `Microsoft.AspNetCore` logging level to `Debug`.
> `dapr run --app-id service-name --app-port #### --components-path ../dapr/components -- dotnet run`

4. Open a terminal at the **reference-architecture** directory and run Tye to launch all services simultaneously.
    ```
    tye run
    ```
5. Alternatively, run Tye in debug mode.
    ```
    tye run --debug *
    ```
    - Set breakpoints in **OrderService**, **CustomerService**.
    - Attach the IDE debugger to **OrderService.dll**, **CustomerService.dll**.
6. Open the Tye dashboard at http://localhost:8000 to inspect service endpoints and view logs.
### Create reference data

7. Create a customer.
   - Open **customers.json** in the **json** folder in CustomerService to copy the JSON for the first customer.
   - Navigate to the customer service and execute a POST with the copied JSON: http://localhost:5064/swagger/

### Create new order

8. Post a new order.
   - Open **order.json** in the **json** folder in OrderService to copy the JSON for a new order.
   - Navigate to the order service and execute a POST with the copied JSON: http://localhost:5214/swagger/
9.  To observe the flow of the Create Order saga, you may wish to run Tye in debug mode and set a breakpoint in the `Create` method of `OrderCommandController`, stepping into `_commandHandler.Handle`.
    - You'll also want to set a breakpoint in the `HandleAsync` method of both `CustomerCreditReserveFulfilledEventHandler`.
    - In addition you can set breakpoints in the `HandleAsync` method of `CustomerCreditReserveRequestedEventHandler` in the customer service.
10. If the saga completed succssfully, the order `State` property should be set to `2` (`Created`). Customer credit should be decremented accordingly.
    - If the saga was unsuccessful due to insufficient credit, then the order `State` property should be set to `0` (`Initial`). Customer credit should be unchanged.
    - If the saga was unsuccessul due to an error condition, the order `State` property may be set to `1` (`Pending`). If this is the case you may need to delete the order record in MongoDB, or simply reset the `State` property of the order to `0` (`Initial`).

## Development Guide

### Introduction

The saga orchestrator lives in the service which begins the saga. Classes which the saga needs are classified according to the following categories:
- Commands
- Dispatchers
- Evaluators
- Handlers

The saga has a single command dispatcher with a `DispatchCommandAsync` method that hands off a `SagaCommand` to a class that extends `ResultDispatchingSagaCommandHandler`.

Some command handlers, such as `SetOrderStateCreatedCommandHandler`, simply update the order `State` using an `IOrderRepository`. Other command handlers, such as `ReserveCustomerCreditCommandHandler`, use an `IEventBus` to publish an event stating that a customer credit reservation has been requested.

The order service has integration event handlers, such as `CustomerCreditReserveFulfilledEventHandler`, which handle integration events via `endpoints.MapDaprEventBus` in `Program`.

Services participating in the saga, such as **CustomerService**, have their own integration event handlers for responsing to events published by `CreateOrderSaga` from the OrderService. The integraton event handler has a reference to a command handler that processes the request and publishes a result back to the event bus via `IEventBus.PublishAsync`. 

The job of a saga orchestrator, such as `CreateOrderSaga`, is to manage a state machine to determine whether to execute the next step in a configured saga, or to execute a series of compensating actions that roll back an operation that spans multiple services. It makes this determination with the help of a result evaluator. Along the way, it persists a snapshot of each step that the saga orchestrator takes.

<p align="center">
  <img width="900" src="images/saga-workflow.png">
</p>

### Integration: Models and Events

1. Create a new class library project called **Integration**.
   - This will contain classes that are shared between a sagas in the Order Service and other services which participate in the saga.
2. Create the following request and response models in a **Models** folder.
   - CustomerCreditReserveRequest
   - CustomerCreditReserveResponse
   - CustomerCreditReleaseRequest
   - CustomerCreditReleaseResponse
3. Create the following integration events in an **Events** folder.
   - CustomerCreditReserveRequested
   - CustomerCreditReserveFulfilled
   - CustomerCreditReleaseRequested
   - CustomerCreditReleaseFulfilled

### Order Service: Create Order Saga

1. Create a new Web API project which will contain one or more sagas.
   - Remove WeatherForecast class and controller.
   ```
   dotnet new webapi --name OrderService
   ```
2. Add NuGet packages.
   - MongoDB.Driver
   - URF.Core.Mongo
   - EventDriven.EventBus.Dapr
   - EventDriven.DependencyInjection.URF.Mongo
   - EventDriven.EventBus.Dapr.EventCache.Mongo
   - AutoMapper.Extensions.Microsoft.DependencyInjection
   - EventDriven.Sagas.Abstractions
   - EventDriven.Sagas.DependencyInjection
   - EventDriven.Sagas.Persistence.Mongo
   - EventDriven.Sagas.Configuration.Mongo
3. Add a **Domain** folder.
   - Add an **OrderAggregate** folder to the **Domain** folder.
   - Place `OrderState`, `OrderItem` and `Order` classes in this folder.
4. The `OrderState` enum serves as a semantic lock, which prevents duplicate sagas for a specific order from taking place while the order is in a pending state.
    ```csharp
    public enum OrderState
    {
        Initial,
        Pending,
        Created
    }
    ```
5. Add a **Repositories** folder at the projet root with an `IOrderRepository` interface and a `OrderRepository` class.
   - `OrderRepository` accepts an `IDocumentRepository<Order>` constructor parameter, which is uses to retrieve and persist `Order` entities to MongoDB.
6. Add a **Sagas** folder to the project root. Place a **CreateOrder** folder within it.
   - Add a `CreateOrderSaga` class to the **CreateOrder** folder.
   - Derive the class from the `PersistableSaga` abstract class.
   - Override the `CheckLock` method to handle `CheckSagaLockCommand`.
   - Add a ctor that accepts `ISagaCommandDispatcher`, `IEnumerable<ISagaCommandResultEvaluator>`.
   - Override the `ExecuteCurrentActionAsync` method, switching on the action command.
    ```csharp
    protected override async Task ExecuteCurrentActionAsync()
    {
        var action = GetCurrentAction();
        if (Entity is Order order)
        {
            switch (action.Command)
            {
                case Commands.CreateOrder:
                    SetActionStateStarted(action);
                    SetActionCommand(action, order);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
                case ReserveCustomerCredit command:
                    command.CustomerId = order.CustomerId;
                    command.CreditRequested = order.OrderItems.Sum(e => e.ProductPrice);
                    SetActionStateStarted(action);
                    SetActionCommand(action);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
                case SetOrderStateCreated:
                    SetActionStateStarted(action);
                    SetActionCommand(action, order);
                    await SagaCommandDispatcher.DispatchCommandAsync(action.Command, false);
                    break;
            }
            return;
        }
        await base.ExecuteCurrentActionAsync();
    }
    ```
    - Override `ExecuteAfterStep` to call `PersistAsync`.
    - Implement `ISagaCommandResultHandler<OrderState>` to handle the command result. Then do the same for the command result handlers for customer credit.
    ```csharp
    public async Task HandleCommandResultAsync(OrderState result, bool compensating)
    {
        SetCurrentActionCommandResult(result);
        await HandleCommandResultForStepAsync<CreateOrderSaga, OrderState, OrderState>(compensating);
    }
    ```
7. Add a **Commands** folder to the **CreateOrder** folder in **Sagas**.
   - Add records for the following commands. Each command extends `SagaCommand`.
     - CreateOrder
     - GetOrderState
     - ReserveCustomerCredit
     - ReleaseCustomerCredit
     - SetOrderStateCreated
     - SetOrderStateInitial
8. Add a **Dispatchers** folder to the **CreateOrder** folder in **Sagas**.
   - Create a `CreateOrderSagaCommandDispatcher` class that extends `SagaCommandDispatcher`.
   - Override `DispatchCommandAsync` to dispatch various commands.
    ```csharp
    public class CreateOrderSagaCommandDispatcher : SagaCommandDispatcher
    {
        public CreateOrderSagaCommandDispatcher(IEnumerable<ISagaCommandHandler> sagaCommandHandlers) :
            base(sagaCommandHandlers)
        {
        }

        public override async Task DispatchCommandAsync(SagaCommand command, bool compensating)
        {
            switch (command.GetType().Name)
            {
                case nameof(Commands.CreateOrder):
                    await DispatchSagaCommandHandlerAsync<Commands.CreateOrder>(command);
                    break;
                case nameof(SetOrderStateInitial):
                    await DispatchSagaCommandHandlerAsync<SetOrderStateInitial>(command);
                    break;
                case nameof(ReserveCustomerCredit):
                    await DispatchSagaCommandHandlerAsync<ReserveCustomerCredit>(command);
                    break;
                case nameof(ReleaseCustomerCredit):
                    await DispatchSagaCommandHandlerAsync<ReleaseCustomerCredit>(command);
                    break;
                case nameof(SetOrderStateCreated):
                    await DispatchSagaCommandHandlerAsync<SetOrderStateCreated>(command);
                    break;
            }
        }
    }
    ```
9. Add a **Handlers** folder to the **CreateOrder** folder in **Sagas**.
   - Add classes for the following:
     - CheckSagaLockCommandHandler
     - CreateOrderCommandHandler
     - ReserveCustomerCreditCommandHandler
     - ReleaseCustomerCreditCommandHandler
     - SetOrderStateCreatedCommandHandler
     - SetOrderStateInitialCommandHandler
10. Add a **Evaluators** folder to the **CreateOrder** folder in **Sagas**.
    - Add classes for the following:
      - ReserveCustomerCreditResultEvaluator
      - SetOrderStateResultEvaluator
11. Add a **Commands** folder to the **OrderAggregate** folder.
    - Add a `StartCreateOrderSaga` record.
        ```csharp
        public record StartCreateOrderSaga(Order Entity) : Command<Order>(Entity);
        ```
    - Place a `StartCreateOrderSagaCommandHandler` class in a **Handlers** folder under **Commands**.
    - Implement `ICommandHandler<Order, StartCreateOrderSaga>`.
    - Inject `IOrderRepository`, `CreateOrderSaga` into the constructor.
    - In the `Handle` method, call `StartSagaAsync` on the saga, then query the order repository to return the newly created order.
    ```csharp
    public class StartCreateOrderSagaCommandHandler :
        ICommandHandler<Order, StartCreateOrderSaga>
    {
        private readonly IOrderRepository _repository;
        private readonly CreateOrderSaga _saga;

        public StartCreateOrderSagaCommandHandler(
            IOrderRepository repository,
            CreateOrderSaga createOrderSaga)
        {
            _repository = repository;
            _logger = logger;
            _saga = createOrderSaga;
        }

        public async Task<CommandResult<Order>> Handle(StartCreateOrderSaga command)
        {
            try
            {
                await _saga.StartSagaAsync(command.Entity);
                var order = await _repository.GetOrderAsync(command.EntityId);
                return order == null
                    ? new CommandResult<Order>(CommandOutcome.NotFound)
                    : new CommandResult<Order>(CommandOutcome.Accepted, order);
            }
            catch (SagaLockedException e)
            {
                return new CommandResult<Order>(CommandOutcome.Conflict, e.ToErrors());
            }
        }
    }
    ```
12. Add a **Handler** folder to an **Integration** folder at the project root.
    - Add a `CustomerCreditReserveFulfilledEventHandler ` class.
    - Override `HandleAsync` to dispatch the command result to the saga when a message is received.
    - Do the same for `CustomerCreditReleaseFulfilledEventHandler`.
    ```csharp
    public class CustomerCreditReserveFulfilledEventHandler : 
        IntegrationEventHandler<CustomerCreditReserveFulfilled>,
        ISagaCommandResultDispatcher<CustomerCreditReserveResponse>
    {
        private readonly ILogger<CustomerCreditReserveFulfilledEventHandler> _logger;
        public Type? SagaType { get; set; } = typeof(CreateOrderSaga);
        public ISagaCommandResultHandler SagaCommandResultHandler { get; set; } = null!;

        public async Task DispatchCommandResultAsync(CustomerCreditReserveResponse commandResult, bool compensating)
        {
            if (SagaCommandResultHandler is ISagaCommandResultHandler<CustomerCreditReserveResponse> handler)
                await handler.HandleCommandResultAsync(commandResult, compensating);
        }
        
        public CustomerCreditReserveFulfilledEventHandler(
            ILogger<CustomerCreditReserveFulfilledEventHandler> logger)
        {
            _logger = logger;
        }

        public override async Task HandleAsync(CustomerCreditReserveFulfilled @event)
        {
            _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReserveFulfilled)}");
            await DispatchCommandResultAsync(new CustomerCreditReserveResponse(
                @event.CustomerCreditReserveResponse.CustomerId,
                @event.CustomerCreditReserveResponse.CreditRequested,
                @event.CustomerCreditReserveResponse.CreditAvailable,
                @event.CustomerCreditReserveResponse.Success
            ), !@event.CustomerCreditReserveResponse.Success);
        }
    }
    ```
13. Create DTO's with an auto mapper profile.
    - Add a **DTO** folder to the project root with `Order` and `OrderItem` DTO's.
      - Include the same properties as the domain classes, but add `Id` and `ETag` from the `Entity` base class.
    - Add a `AutoMapperProfile` class to a **Mapping** folder at the project root.
    ```csharp
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Entities.Order, Order>();
            CreateMap<Entities.Order, Order>().ReverseMap();
            CreateMap<Entities.OrderItem, OrderItem>();
            CreateMap<Entities.OrderItem, OrderItem>().ReverseMap();
        }
    }
    ```
14. Add controllers to the **Controllers** folder.
    - Add a `OrderQueryController` class that uses an `IOrderRepository` to retrieve orders, mapping the result to DTO's using an `IMapper`.
    - Add a `OrderCommandController` class that accepts a constructor parameter of `StartCreateOrderSagaCommandHandler` to handle a `StartCreateOrderSaga` command.
15. Configure services and endpoints in `Program`.
    - Automapper
    ```csharp
    builder.Services.AddAutoMapper(typeof(Program));
    ```
    - Repositories and database settings
    ```csharp
    builder.Services.AddSingleton<IOrderRepository, OrderRepository>();
    builder.Services.AddMongoDbSettings<OrderDatabaseSettings, Order>(builder.Configuration);
    builder.Services.AddMongoDbSettings<SagaConfigDatabaseSettings, SagaConfigurationDto>(builder.Configuration);
    builder.Services.AddMongoDbSettings<SagaSnapshotDatabaseSettings, SagaSnapshotDto>(builder.Configuration);
    ```
    - Command handlers
    ```csharp
    builder.Services.AddCommandHandlers();
    ```
    - App settings
    ```csharp
    builder.Services.AddAppSettings<SagaConfigSettings>(builder.Configuration);
    ```
    - Sagas
    ```csharp
    builder.Services.AddSaga<CreateOrderSaga, CreateOrderSagaCommandDispatcher,
        SagaConfigRepository, SagaSnapshotRepository, SagaConfigSettings>(
        builder.Configuration);
    ```
    - Event Bus and event handlers
    ```csharp
    builder.Services.AddDaprEventBus(builder.Configuration, true);
    builder.Services.AddDaprMongoEventCache(builder.Configuration);
    builder.Services.AddSingleton<CustomerCreditReserveFulfilledEventHandler>();
    ```
    - Map Dapr Event Bus subscribers
    ```csharp
    app.UseRouting();
    app.UseCloudEvents();
    app.UseEndpoints(endpoints =>
    {
        endpoints.MapControllers();
        endpoints.MapSubscribeHandler();
        endpoints.MapDaprEventBus(eventBus =>
        {
            var customerCreditReservedEventHandler = app.Services.GetRequiredService<CustomerCreditReserveFulfilledEventHandler>();
            eventBus.Subscribe(customerCreditReservedEventHandler, nameof(CustomerCreditReserveFulfilled), "v1");
        });
    });
    ```

### Customer Service: Handle credit reservation requested event

1. Create a new Web API project for a customer service.
   - Remove WeatherForecast class and controller.
   ```
   dotnet new webapi --name CustomerService
   ```
2. Add a `Customer` entity to a **Domain/CustomerAggregate** folder.
   - Add the following commands to a **Commands** folder.
     - ReserveCredit
     - ReleaseCredit
   - Add the following events to an **Events** folder.
     - CreditReserveSucceeded
     - CreditReserveFailed
     - CreditReleased
   - Update `Customer` to implement:
     - `ICommandProcessor<ReserveCredit>`
     - `IEventApplier<CreditReserveSucceeded>`
     - `ICommandProcessor<ReleaseCredit, CreditReleased>`
     - `IEventApplier<CreditReleased>`
    ```csharp
    public IDomainEvent Process(ReserveCredit command)
    {
        // If customer has sufficient credit, return CreditReserveSucceeded event
        if (CreditAvailable >= command.CreditRequested)
            return new CreditReserveSucceeded(command.EntityId, command.CreditRequested) { EntityETag = ETag };
        // Otherwise, return CreditReserveFailed event
        return new CreditReserveFailed(command.EntityId, command.CreditRequested) { EntityETag = ETag };
    }
    ```
3. Add a `CustomerCreditReserveRequestedEventHandler` class to an **Integration/Handlers** folder.
   - Inject `ICommandHandler<Customer, ReserveCredit>` into the constructor.
   - Extend `IntegrationEventHandler<CustomerCreditReserveRequested>`.
   - Override `HandleAsync` to pass a `ReserveCredit` command to the command handler.
    ```csharp
    public override async Task HandleAsync(CustomerCreditReserveRequested @event)
    {
        _logger.LogInformation("Handling event: {EventName}", $"v1.{nameof(CustomerCreditReserveRequested)}");

        var command = new ReserveCredit(
            @event.CustomerCreditReserveRequest.CustomerId,
            @event.CustomerCreditReserveRequest.CreditReserved);
        await _commandHandler.Handle(command);
    }
    ```
    - Repeat with a `CustomerCreditReserveReleaseEventHandler` class.
4. Create a `CustomerCommandHandler` class in a **Domain/CustomerAggregate/Handlers** folder.
   - Inject an `IEventBus` into the constructor.
   - Create a private `PublishCreditReservedResponse` helper method to publish a `CustomerCreditReserveFulfilled` integration event to the event bus.
    ```csharp
    private async Task<CommandResult<Customer>> PublishCreditReservedResponse(Customer customer, decimal creditRequested, bool success)
    {
        try
        {
            var @event = new CustomerCreditReserveFulfilled(
                new CustomerCreditReserveResponse(customer.Id, creditRequested,
                    customer.CreditAvailable, success));
            await _eventBus.PublishAsync(@event,
                nameof(CustomerCreditReserveFulfilled), "v1");
            return new CommandResult<Customer>(CommandOutcome.Accepted, customer);
        }
        catch (SchemaValidationException e)
        {
            _logger.LogError("{Message}", e.Message);
            return new CommandResult<Customer>(CommandOutcome.NotHandled);
        }
    }
    ```
   - Implement `ICommandHandler<Customer, ReserveCredit>` to process the `ReserveCredit` command, pubish the `CustomerCreditReserveFulfilled` event, and persist the credit reservation.
    ```csharp
    public async Task<CommandResult<Customer>> Handle(ReserveCredit command)
    {
        // Process command to determine if customer has sufficient credit
        var customer = await _repository.GetAsync(command.EntityId);
        if (customer == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = customer.Process(command);

        // Return if credit reservation unsuccessful
        if (domainEvent is not CreditReserveSucceeded succeededEvent)
            return await PublishCreditReservedResponse(customer, command.CreditRequested, false);

        // Apply events to reserve credit
        customer.Apply(succeededEvent);

        Customer? entity = null;
        CommandResult<Customer> result;
        try
        {
            // Persist credit reservation
            entity = await _repository.UpdateAsync(customer);
            if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            result = await PublishCreditReservedResponse(entity, command.CreditRequested, true);
            
            // Reverse persistence if publish is unsuccessful
            if (result.Outcome != CommandOutcome.Accepted)
            {
                var creditReleasedEvent = customer.Process(
                    new ReleaseCredit(customer.Id, command.CreditRequested));
                customer.Apply(creditReleasedEvent);
                entity = await _repository.UpdateAsync(customer);
                if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
            }
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError("{Message}", e.Message);
            result = await PublishCreditReservedResponse(entity ?? customer, command.CreditRequested, false);
        }

        return result;
    }
    ```
