using EventDriven.EventBus.Abstractions;
using Integration.Models;

namespace Integration.Events;

public record CustomerCreditReleaseFulfilled(CustomerCreditReleaseResponse CustomerCreditReleaseResponse) : IntegrationEvent;