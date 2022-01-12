using EventDriven.DDD.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactoryFakes;

public class FakeEntity : Entity
{
    public string State { get; set; } = null!;
}