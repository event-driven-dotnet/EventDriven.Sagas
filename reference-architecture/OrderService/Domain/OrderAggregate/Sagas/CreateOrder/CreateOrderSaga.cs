using EventDriven.Sagas.Abstractions;

namespace OrderService.Domain.OrderAggregate.Sagas.CreateOrder;

public class CreateOrderSaga : Saga
{
    public override Task StartSagaAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override Task ExecuteCurrentActionAsync()
    {
        throw new NotImplementedException();
    }

    protected override Task ExecuteCurrentCompensatingActionAsync()
    {
        throw new NotImplementedException();
    }
}