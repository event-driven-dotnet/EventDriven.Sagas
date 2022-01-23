using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record CustomerCreditReserveRequested(CustomerCreditReserveRequest CustomerCreditReserveRequest) : IntegrationEvent;