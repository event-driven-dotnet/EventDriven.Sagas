using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record ProductInventoryReserveFulfilled(ProductInventoryReserveResponse ProductInventoryReserveResponse) : IntegrationEvent;