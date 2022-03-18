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
  - ReadMe: [Order Service Sagas: User Acceptance Tests](test/OrderService.Sagas.Specs/ReadMe.md)

## Reference Architecture
- **SagaConfigDefinition**: Saga Configuration Definition
  - Provides method that returns a `SagaConfigurationDto` containing Create Order Saga steps, actions and commands.
- **Common**: Contains models and events that are exchanged between services taking part in the saga. Also contains behaviors for cross-cutting concerns such as validation or logging.
- **OrderService**: Contains `CreateOrderSaga` with dispatchers, commands, handlers and evaluators for orchestrating a saga with updates that span multiple services.
- **CustomerService**: Contains handlers for processing integration events that update the customer data store when credit is reserved or released.

## Introduction

The purpose of the [Saga pattern](https://microservices.io/patterns/data/saga.html) is to enable **atomic operations which span multiple microservices**, each of which have their own private data store. Each service may perform local transactions against their data store, but distributed transactions in the classical sense are impractical in a miroservices architecture, where holding locks for the two-phase protocol would impair performance and scalability.

The Saga pattern consists of a series of steps with actions and corresponding compensating actions. Steps have a sequence number, and actions have a command which is executed by a participating microservice and has an expected result. The saga can then use the result as a basis for executing the next step or initiating a rollback with a series of compensating actions. Sagas can be configured and persisted via a durable data store.

The Saga orchestrator coordinates the entire process by telling each participant what to do and invoking the next participant in either moving the saga forward or rolling it backwards. The orchestrator can run in the same process as the initiating microservice, and it can also communicate with other microservices asynchronously through an event bus abstraction.

![Saga Orchestration](images/saga-orchestration.png)

## Running the Sample

### Generate the saga configuration

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


### Run services with Tye and Dapr

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
    - Set breakpoints in **OrderService**, **CustomerService**, **InventoryService**.
    - Attach the IDE debugger to **OrderService.dll**, **CustomerService.dll**, **InventoryService.dll**.
6. Open the Tye dashboard at http://localhost:8000 to inspect service endpoints and view logs.

### Start the saga by creating a new order

7. First create a new customer.
   - Open **customers.json** in the **json** folder in CustomerService to copy the JSON for the first customer.
   - Navigate to the customer service and execute a POST with the copied JSON: http://localhost:5064/swagger/
8. Then create a new order.
   - Open **order.json** in the **json** folder in OrderService to copy the JSON for a new order.
   - Navigate to the order service and execute a POST with the copied JSON: http://localhost:5214/swagger/
9.  To observe the flow of the Create Order saga, you should run Tye in debug mode. Set the following breakpoints in the **Order** service.
    - **Contollers**: `OrderCommandController`.
      - `Create` method.
        - Step into `_commandHandler.Handle`.
    - **Sagas**: `CreateOrderSaga`.
      - `ExecuteCurrentActionAsync` method.
        - Step into `SagaCommandDispatcher.DispatchCommandAsync`.
      - `HandleCommandResultAsync` methods.
        - Step into `HandleCommandResultForStepAsync`.
    - **Integration/Handlers**: `CustomerCreditReserveFulfilledEventHandler`.
      - `HandleAsync` method.
        - Step into `handler.HandleCommandResultAsync`.
    - **Integration/Handlers**: `ProductInventoryReserveFulfilledEventHandler`.
      - `HandleAsync` method.
        - Step into `handler.HandleCommandResultAsync`.
10. Set breakpoints in the **Customer** and **Inventory** services.
    - **Integration/Handlers**: `ProductInventoryReserveRequestedEventHandler`.
      - `HandleAsync` method.
        - Step into `_commandHandler.Handle`.
    - **Integration/Handlers**: `CustomerCreditReserveRequestedEventHandler`.
      - `HandleAsync` method.
        - Step into `_commandHandler.Handle`.
11. If the saga completed succssfully, the order `State` property should be set to `2` (`Created`). Customer credit and product inventory should be reduced accordingly.
    - If the saga was unsuccessful due to insufficient credit, then the order `State` property should be set to `0` (`Initial`) and the customer credit should be unchanged.
    - If the saga was unsuccessul due to an error condition, the order `State` property may be set to `1` (`Pending`). If this is the case you may need to delete the order record in MongoDB, or reset the `State` property of the order to `0` (`Initial`).

## Development Guide

### Overview

Saga orchestration begins with the service that *initiates the saga*. In the reference architecture this is the Order service. Sagas consist of **steps**, **actions** and **commands**, which are arranged in a saga **configuration** that is read from a durable store.

The first step is to create a saga orchestrator class that inherits from `PersistableSaga`, which ensures that snapshots of the saga are persisted as each step is executed. In the Order service this is the `CreateOrderSaga` class.

The following diagram illustrates how various classes are used in the execution of a saga.

<p align="center">
  <img width="900" src="images/saga-workflow.png">
</p>


In the **reference-architecture/SagaConfigDefintiions** project, the `CreateOrderSagaConfigDefinition` class implements `ISagaConfigDefinition` with a `CreateSagaConfig` that accepts a saga config id and returns a `SagaConfigurationDto` that has steps with actions and compensating actions, each with a saga command that is serialized to a JSON string.

In the saga orchestrator class you will override two methods: `ExecuteCurrentActionAsync`, for executing actions when the saga is started, and `ExecuteCurrentCompensatingActionAsync`, for executing compensating actions when the saga is rolled back. Each of these uses a **dispatcher** to execute a **command** that is defined in the saga configuration.

The dispatcher sends the command to a **handler** that uses a **repository** to update entity state and then publishes an **integration event** to the **event bus**. For example, the `CreateOrderSagaCommandDispatcher` dispatches a `ReserveCustomerCredit` command to the `ReserveCustomerCreditCommandHandler`, which publishes a `CustomerCreditReserveRequest` integration event to an `IEventBus`.

Other services, such as Customer and Inventory, subscribe to **integration events** and use their own command handlers to process requests and publish responses back to the event bus.

The service that initiated the saga has its own **integration event handlers** to handle responses published by external services. For example, the Order service has a `CustomerCreditReserveFulfilledEventHandler` that dispatches the result to `CreateOrderSaga`, which implements `ISagaCommandResultHandler<CustomerCreditReserveResponse>` with a `HandleCommandResultAsync` method that updates the saga state machine so that it knows whether to execute the next step in the saga or to roll back the saga with a series of compensating actions.

In the case of `CreateOrderSaga`, it is configured to perform the following steps:
- First, change the order state from `Intial` to `Pending` in order to lock the saga and prevent changes to the order while the saga is being executed.
- Second, reserve customer credit. If the customer has insufficient credit for the order, initiate a rollback of the previous step by setting the order state to `Initial`.
- Third, reserve product inventory. If there is insifficient inventory to process the order, initiate a rollback of previous steps in the saga, starting with a release of the credit that was previously reserved for the order.
- Fourth, set the order state to `Created`.

### Steps

For step-by-step instructions on how to build a saga orchestration that executes updates across multiple services, please see the [Development Guide](DevelopmentGuide.md).