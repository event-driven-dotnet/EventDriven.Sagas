using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record ProductInventoryReleaseRequested(ProductInventoryReleaseRequest ProductInventoryReleaseRequests) : IntegrationEvent;