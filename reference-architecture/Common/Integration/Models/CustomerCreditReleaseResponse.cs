namespace Common.Integration.Models;

public record CustomerCreditReleaseResponse(Guid CustomerId, decimal CreditRequested, decimal CreditRemaining, bool Success);