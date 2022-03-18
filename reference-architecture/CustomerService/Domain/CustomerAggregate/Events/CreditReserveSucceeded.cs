namespace CustomerService.Domain.CustomerAggregate.Events;

public record CreditReserveSucceeded(Guid EntityId, decimal AmountRequested) : CreditReserved(EntityId, AmountRequested);