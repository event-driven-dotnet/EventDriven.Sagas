using EventDriven.EventBus.Abstractions;
using Common.Integration.Models;

namespace Common.Integration.Events;

public record ProductInventoryReserveFulfilled(ProductInventoryReserveResponse ProductInventoryReserveResponse) : IntegrationEvent;