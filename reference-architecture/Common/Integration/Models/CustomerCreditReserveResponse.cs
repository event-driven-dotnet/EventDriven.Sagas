namespace Common.Integration.Models;

public record CustomerCreditReserveResponse(Guid CustomerId, decimal CreditRequested, decimal CreditAvailable, bool Success);