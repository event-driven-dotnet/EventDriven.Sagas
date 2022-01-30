﻿using EventDriven.Sagas.Abstractions.Commands;
using Integration.Models;

namespace OrderService.Sagas.Commands;

public record ReserveCustomerCredit(Guid CustomerId, decimal CreditRequested) :
    SagaCommand<CustomerCreditReserveResponse, CustomerCreditReserveResponse>
{
    public Guid CustomerId { get; set; } = CustomerId;
    public decimal CreditRequested { get; set; } = CreditRequested;
}