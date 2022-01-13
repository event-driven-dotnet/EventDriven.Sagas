using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using OrderService.Domain.OrderAggregate.Sagas;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands.Handlers;

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

        try
        {
            // Add or update order
            var order = await _repository.AddUpdateOrderAsync(command.Order);
            if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound, command.Order);
            
            // Start create order saga
            await _saga.StartSagaAsync(command.EntityId);
            return new CommandResult<Order>(CommandOutcome.Accepted, order);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new CommandResult<Order>(CommandOutcome.NotFound, command.Order);
        }
    }
}