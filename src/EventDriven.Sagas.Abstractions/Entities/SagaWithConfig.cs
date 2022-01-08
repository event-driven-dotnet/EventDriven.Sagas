using EventDriven.Sagas.Abstractions.Configuration;
using EventDriven.Sagas.Abstractions.Repositories;

namespace EventDriven.Sagas.Abstractions.Entities;

/// <summary>
/// Enables the execution of atomic operations which span multiple services.
/// </summary>
/// <typeparam name="TEntity">Entity type.</typeparam>
public abstract class SagaWithConfig<TEntity> : Saga<TEntity>
{
    /// <summary>
    /// Optional saga configuration identifier.
    /// </summary>
    public SagaConfigurationOptions? SagaConfigOptions { get; set; }

    /// <summary>
    /// Saga configuration repository.
    /// </summary>
    public ISagaConfigRepository? SagaConfigRepository { get; set; }

    /// <summary>
    /// Configure saga steps.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual async Task ConfigureSteps()
    {
        if (SagaConfigOptions?.SagaConfigId != null && SagaConfigRepository != null)
        {
            var sagaConfig = await SagaConfigRepository
                .GetSagaConfigurationAsync(SagaConfigOptions.SagaConfigId.GetValueOrDefault());
            if (sagaConfig == null)
                throw new Exception($"Saga configuration with id '{SagaConfigOptions.SagaConfigId}' not present in Saga Configuration Repository.");
            // TODO: Use mapper
            Steps = new List<SagaStep>(); // sagaConfig.Steps;
        }
    }

    /// <inheritdoc />
    public override async Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        // Set steps from config
        await ConfigureSteps();
        
        // Start saga
        await base.StartSagaAsync(cancellationToken);
    }
}