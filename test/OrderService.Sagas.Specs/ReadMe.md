# Order Service Sagas BDD Tests

SpecFlow tests for Order Service sagas.

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
- [Install Tye](https://github.com/dotnet/tye/blob/main/docs/getting_started.md)
- [Specflow](https://specflow.org/) IDE Plugin
  - [Visual Studio](https://docs.specflow.org/projects/getting-started/en/latest/GettingStarted/Step1.html)
  - [JetBrains Rider](https://docs.specflow.org/projects/specflow/en/latest/Rider/rider-installation.html)

## Usage

### Option 1: Run Tye Independently (Recommended)

1. Open appsettings.json and set `StartTyeProcess` to `false`.
2. Run Tye from a terminal at the SpecFlow project root.
    ```
    tye run
    ```
3. Alternatively, run Tye in debug mode.
    ```
    tye run --debug *
    ```
    - Set breakpoints in OrderService and CustomerService.
    - Attach the IDE debugger to both OrderService.dll and CustomerService.dll.
4. Run the SpecFlow tests using the IDE test runner.
   - You should hit breakpoints in OrderService and CustomerService.

### Option 2: Run Tye with SpecFlow

1. Open appsettings.json and set `StartTyeProcess` to `true`.
2. Run the SpecFlow tests using the IDE test runner.
3Alternatively, run tests from a terminal at the SpecFlow project root.
    ```
    dotnet test
    ```

