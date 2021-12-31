using EventDriven.DDD.Abstractions.Commands;

namespace OrderService.Domain.OrderAggregate.Commands;

public class OrderCommandHandler :
    ICommandHandler<Order, CreateOrder>,
    ICommandHandler<Order, SetOrderState>
{
    private Order _order = null!;
    private readonly ILogger<OrderCommandHandler> _logger;

    public OrderCommandHandler(
        ILogger<OrderCommandHandler> logger)
    {
        _logger = logger;
    }

    public Task<CommandResult<Order>> Handle(CreateOrder command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
        _order = command.Order;

        // TODO: Start create order saga
        throw new NotImplementedException();
    }

    public Task<CommandResult<Order>> Handle(SetOrderState command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
        _order.State = command.Payload;

        // TODO: Set order state
        throw new NotImplementedException();
    }
}