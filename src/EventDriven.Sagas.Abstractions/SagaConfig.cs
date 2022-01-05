using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace EventDriven.Sagas.Abstractions;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
public abstract class SagaConfig : Saga
{
    /// <summary>
    /// SagaConfig constructor.
    /// Note: To use <see cref="IServiceCollection"/>.AddSaga, inheritors must have a parameterless constructor. 
    /// </summary>
    // ReSharper disable once EmptyConstructor
    protected SagaConfig()
    {
    }
    
    /// <summary>
    /// Saga configuration repository.
    /// </summary>
    public ISagaConfigRepository? SagaConfigRepository { get; set; }

    /// <summary>
    /// Saga command dispatcher.
    /// </summary>
    public ISagaCommandDispatcher? SagaCommandDispatcher { get; set; }

    /// <summary>
    /// Command result evaluator.
    /// </summary>
    public ICommandResultEvaluator? CommandResultEvaluator { get; set; }

    /// <inheritdoc />
    protected override async Task ConfigureSteps()
    {
        if (SagaConfigOptions?.SagaConfigId != null && SagaConfigRepository != null)
        {
            var sagaConfig = await SagaConfigRepository
                .GetSagaConfigurationAsync(SagaConfigOptions.SagaConfigId.GetValueOrDefault());
            if (sagaConfig != null) Steps = sagaConfig.Steps;
        }
    }
}