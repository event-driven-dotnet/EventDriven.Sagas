namespace EventDriven.Sagas.Abstractions.Configuration;

/// <summary>
/// Saga configuration options.
/// </summary>
public class SagaConfigurationOptions
{
    /// <summary>
    /// Saga configuration identifier.
    /// </summary>
    public Guid? SagaConfigId { get; set; }
}