namespace EventDriven.Sagas.Configuration.Abstractions;

/// <summary>
/// Saga settings interface.
/// </summary>
public interface ISagaConfigSettings
{
    /// <summary>
    /// Saga config identifier.
    /// </summary>
    public Guid SagaConfigId { get; set; }
}