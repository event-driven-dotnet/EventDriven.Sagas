namespace Integration.Models;

public record CustomerCreditReleaseResponse(Guid CustomerId, decimal CreditReleased, decimal CreditRemaining);