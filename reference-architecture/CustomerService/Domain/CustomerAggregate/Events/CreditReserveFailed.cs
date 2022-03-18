namespace CustomerService.Domain.CustomerAggregate.Events;

public record CreditReserveFailed(Guid EntityId, decimal AmountRequested) : CreditReserved(EntityId, AmountRequested);