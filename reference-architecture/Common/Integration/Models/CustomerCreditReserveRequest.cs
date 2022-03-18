namespace Common.Integration.Models;

public record CustomerCreditReserveRequest(Guid CustomerId, decimal CreditReserved);