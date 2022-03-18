using EventDriven.EventBus.Abstractions;
using Common.Integration.Models;

namespace Common.Integration.Events;

public record ProductInventoryReleaseFulfilled(ProductInventoryReleaseResponse ProductInventoryReleaseResponse) : IntegrationEvent;