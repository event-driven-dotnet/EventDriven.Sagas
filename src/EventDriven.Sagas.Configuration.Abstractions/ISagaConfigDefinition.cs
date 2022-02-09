using EventDriven.Sagas.Configuration.Abstractions.DTO;

namespace EventDriven.Sagas.Configuration.Abstractions;

/// <summary>
/// Saga configuration definition.
/// </summary>
public interface ISagaConfigDefinition
{
    /// <summary>
    /// Create saga configuration definition.
    /// </summary>
    /// <param name="id">Saga configuration </param>
    /// <returns></returns>
    SagaConfigurationDto CreateSagaConfig(Guid id);
}