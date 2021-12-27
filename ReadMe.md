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

The purpose of the [Saga pattern](https://microservices.io/patterns/data/saga.html) is to enable the equivalent of atomic operations which span multiple microservices, each of which have their own private data store. Each service may perform local transactions against their data store, but distributed transactions in the classical sense are impractical in a miroservices architecture, where holding locks for the two-phase protocol would inhibit performance and scalability.

The Saga pattern consists of a series of steps with actions and corresponding compensating actions. Steps have Command, Outcome and Timeout properties and are assigned a sequence number. The command is executed by a participating microservice, which sets the outcome. The outcome indicates success or failure, which the saga can then use as a basis for invoking the command of the next step or to initiate a rollback with a series of compensating actions. Sagas can be configured and persisted via a durable data store.

The Saga Orchestrator coordinates the entire process by telling each participant what to do and invoking the next participant in either moving the saga forward or rolling it backwards. The orchestrator can run in the same process as the initiating microservice, and it can also communicate with other microservices asynchronously through an event bus abstraction.