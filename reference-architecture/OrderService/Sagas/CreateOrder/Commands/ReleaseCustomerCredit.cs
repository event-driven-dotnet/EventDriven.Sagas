using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.CreateOrder.Commands;

public record ReleaseCustomerCredit(Guid CustomerId, decimal CreditReleased) :
    SagaCommand<CustomerCreditReleaseResponse, CustomerCreditReleaseResponse>
{
    public Guid CustomerId { get; set; } = CustomerId;
    public decimal CreditReleased { get; set; } = CreditReleased;
}