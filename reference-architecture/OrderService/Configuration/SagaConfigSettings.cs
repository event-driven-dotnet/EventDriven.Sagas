using EventDriven.Sagas.Abstractions.Configuration;

namespace OrderService.Configuration;

public class SagaConfigSettings : ISagaConfigSettings
{
    public Guid SagaConfigId { get; set; }
}