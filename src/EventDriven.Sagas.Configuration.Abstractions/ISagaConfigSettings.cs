namespace EventDriven.Sagas.Configuration.Abstractions;

/// <summary>
/// Saga settings interface.
/// </summary>
public interface ISagaConfigSettings
{
    /// <summary>
    /// Saga config identifier.
    /// </summary>
    public Guid? SagaConfigId { get; set; }
    
    /// <summary>
    /// Override lock check.
    /// </summary>
    public bool OverrideLockCheck { get; set; }

    /// <summary>
    /// Enable saga snapshots. Defaults to true.
    /// </summary>
    public bool EnableSagaSnapshots { get; set; }
}