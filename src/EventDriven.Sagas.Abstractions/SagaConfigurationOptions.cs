namespace EventDriven.Sagas.Abstractions;

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