namespace Integration.Models;

public record CustomerCreditReleaseRequest(Guid CustomerId, decimal CreditReleased);