using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record CustomerCreditReleaseRequested(CustomerCreditReleaseRequest CustomerCreditReleaseRequest) : IntegrationEvent;