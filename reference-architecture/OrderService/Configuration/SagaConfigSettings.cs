using EventDriven.Sagas.Abstractions;

namespace OrderService.Configuration;

public class SagaConfigSettings : ISagaConfigSettings
{
    public Guid SagaConfigId { get; set; }
}