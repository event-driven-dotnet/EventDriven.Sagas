using EventDriven.DDD.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Sagas.CreateOrder;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands;

public class OrderCommandHandler :
    ICommandHandler<Order, CreateOrder>,
    ICommandHandler<Order, GetOrderState>,
    ICommandHandler<Order, SetOrderStatePending>
{
    //private Order _order = null!;
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
        var order = await _repository.AddOrder(command.Order);

        // Start saga to create an order
        await _createOrderSaga.StartSagaAsync();
        return new CommandResult<Order>(CommandOutcome.Accepted, order);
    }

    public async Task<CommandResult<Order>> Handle(GetOrderState command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(GetOrderState));
        var order = await _repository.GetOrder(command.EntityId);
        return new CommandResult<Order>(CommandOutcome.Accepted, order);
    }

    public async Task<CommandResult<Order>> Handle(SetOrderStatePending command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStatePending));

        var order = await _repository.GetOrder(command.EntityId);
        var updatedOrder = await _repository.UpdateOrderState(order, OrderState.Pending);
        await _createOrderSaga.ProcessCommandResultAsync(updatedOrder, false);
        return new CommandResult<Order>(CommandOutcome.Accepted, updatedOrder);
    }
}