using EventDriven.DDD.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.Commands;

public record ReleaseCredit(Guid EntityId, decimal CreditReleased) : ICommand;