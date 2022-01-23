using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record CustomerCreditReserveFulfilled(CustomerCreditReserveResponse CustomerCreditReserveResponse) : IntegrationEvent;