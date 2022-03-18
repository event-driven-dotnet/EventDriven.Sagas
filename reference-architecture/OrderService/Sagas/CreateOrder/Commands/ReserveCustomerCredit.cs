using Common.Integration.Models;
using EventDriven.Sagas.Abstractions.Commands;

namespace OrderService.Sagas.CreateOrder.Commands;

public record ReserveCustomerCredit(Guid CustomerId, decimal CreditRequested) :
    SagaCommand<CustomerCreditReserveResponse, CustomerCreditReserveResponse>
{
    public Guid CustomerId { get; set; } = CustomerId;
    public decimal CreditRequested { get; set; } = CreditRequested;
}