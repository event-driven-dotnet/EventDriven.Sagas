using EventDriven.CQRS.Abstractions.Queries;
using OrderService.Domain.OrderAggregate.Queries;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.QueryHandlers;

public class GetOrderStateHandler : IQueryHandler<GetOrderState, OrderState?>
{
    private readonly IOrderRepository _repository;

    public GetOrderStateHandler(
        IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<OrderState?> Handle(GetOrderState query, CancellationToken cancellationToken)
    {
        var result = await _repository.GetOrderStateAsync(query.Id);
        return result;
    }
}