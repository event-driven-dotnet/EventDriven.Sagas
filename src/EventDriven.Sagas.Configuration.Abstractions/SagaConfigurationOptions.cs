namespace EventDriven.Sagas.Configuration.Abstractions;

/// <summary>
/// Saga configuration options.
/// </summary>
public class SagaConfigurationOptions
{
    /// <summary>
    /// Saga configuration identifier.
    /// </summary>
    public Guid? SagaConfigId { get; set; }

    /// <summary>
    /// If true override lock check.
    /// </summary>
    public bool OverrideLockCheck { get; set; }
}