using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record ProductInventoryReserveRequested(ProductInventoryReserveRequest ProductInventoryReserveRequest) : IntegrationEvent;