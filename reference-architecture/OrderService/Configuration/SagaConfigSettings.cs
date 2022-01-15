using EventDriven.Sagas.Configuration.Abstractions;

namespace OrderService.Configuration;

public class SagaConfigSettings : ISagaConfigSettings
{
    public Guid SagaConfigId { get; set; }

    public bool OverrideLockCheck { get; set; }
}