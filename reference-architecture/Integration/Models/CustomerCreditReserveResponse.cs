namespace Integration.Models;

public record CustomerCreditReserveResponse(Guid CustomerId, decimal CreditReserved, decimal CreditRemaining);