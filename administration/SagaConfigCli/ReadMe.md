# Saga Configuration CLI

Command line interface for saga configuration.

## Preparation

1. Create a **SagaConfigDefinitions** class library containing a class that implements `ISagaConfigDefinition`.
2. Flesh out the `CreateSagaConfig` by returning a new `SagaConfigurationDto` with steps that are a `List<SagaStepDto>`.
3. Add a **json** folder in which to place the saga definition JSON file.

## Installation


1. Install the **sagaconfig** CLI tool globally.
    ```
    dotnet tool install -g EventDriven.Sagas.SagaConfig.CLI --version 1.0.0-beta1
    ```

## Usage

1. Navigate to the root of the **SagaConfigDefinitions** project.
    ```
    cd <path>/SagaConfigDefinitions
    ```
2. Run the `sagaconfig` command, passing required parameters.
   - Specify a Guid as the `-id` parameter for the Saga Config Id.
   - Specify a relative path to the location of the SagaConfigDefinitions.dll file for the `-p` parameter.
   - Specify the name of the **json** folder for the `-j` parameter.
   - Omit the `-uri` parameter to save a config JSON file without posting to the SagaConfig Service.
    ```
    sagaconfig -id d89ffb1e-7481-4111-a4dd-ac5123217293 -p bin/Debug/net6.0 -j json
    ```
   - Include the `-uri` parameter to save a config JSON file and post to the SagaConfig Service.
     - First run the SagaConfigService project.
    ```
    sagaconfig -id d89ffb1e-7481-4111-a4dd-ac5123217293 -p bin/Debug/net6.0 -j json -uri http://localhost:5256/api/sagaconfig/
    ```
