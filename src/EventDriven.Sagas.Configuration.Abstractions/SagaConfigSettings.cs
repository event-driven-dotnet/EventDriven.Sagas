namespace EventDriven.Sagas.Configuration.Abstractions;

/// <inheritdoc />
public abstract class SagaConfigSettings : ISagaConfigSettings
{
    /// <inheritdoc />
    public Guid? SagaConfigId { get; set; }

    /// <inheritdoc />
    public bool OverrideLockCheck { get; set; }

    /// <inheritdoc />
    public bool EnableSagaSnapshots { get; set; } = true;
}