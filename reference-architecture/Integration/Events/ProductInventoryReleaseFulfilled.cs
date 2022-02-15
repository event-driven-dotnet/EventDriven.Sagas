using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record ProductInventoryReleaseFulfilled(ProductInventoryReleaseResponse ProductInventoryReleaseResponse) : IntegrationEvent;