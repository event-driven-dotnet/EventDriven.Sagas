using EventDriven.DDD.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Sagas.CreateOrder;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands;

public class CreateOrderCommandHandler :
    ICommandHandler<Order, CreateOrder>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;
    private readonly CreateOrderSaga _saga;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        CreateOrderSaga createOrderSaga,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
        _saga = createOrderSaga;
    }

    public async Task<CommandResult<Order>> Handle(CreateOrder command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
        var order = await _repository.AddOrderAsync(command.Order);

        // Start saga to create an order
        await _saga.StartSagaAsync();
        return new CommandResult<Order>(CommandOutcome.Accepted, order);
    }
}