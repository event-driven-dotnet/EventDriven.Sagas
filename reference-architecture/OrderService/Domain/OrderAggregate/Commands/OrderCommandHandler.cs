using EventDriven.DDD.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Sagas.CreateOrder;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands;

public class OrderCommandHandler :
    ICommandHandler<Order, CreateOrder>,
    ICommandHandler<Order, SetOrderStatePending>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderCommandHandler> _logger;
    private readonly CreateOrderSaga _createOrderSaga;

    public OrderCommandHandler(
        IOrderRepository repository,
        CreateOrderSaga createOrderSaga,
        ILogger<OrderCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
        _createOrderSaga = createOrderSaga;
    }

    public async Task<CommandResult<Order>> Handle(CreateOrder command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
        var order = await _repository.AddOrderAsync(command.Order);

        // Start saga to create an order
        // await _createOrderSaga.StartSagaAsync();
        return new CommandResult<Order>(CommandOutcome.Accepted, order);
    }

    public async Task<CommandResult<Order>> Handle(SetOrderStatePending command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStatePending));

        var order = await _repository.GetOrderAsync(command.EntityId);
        if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);
        var updatedOrder = await _repository.UpdateOrderStateAsync(order, OrderState.Pending);
        // await _createOrderSaga.ProcessCommandResultAsync(updatedOrder, false);
        return new CommandResult<Order>(CommandOutcome.Accepted, updatedOrder);
    }
}