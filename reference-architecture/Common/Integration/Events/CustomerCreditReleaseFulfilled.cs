using EventDriven.EventBus.Abstractions;
using Common.Integration.Models;

namespace Common.Integration.Events;

public record CustomerCreditReleaseFulfilled(CustomerCreditReleaseResponse CustomerCreditReleaseResponse) : IntegrationEvent;