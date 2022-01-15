using EventDriven.DDD.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Sagas;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands.Handlers;

public class StartCreateOrderSagaCommandHandler :
    ICommandHandler<Order, StartCreateOrderSaga>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<StartCreateOrderSagaCommandHandler> _logger;
    private readonly CreateOrderSaga _saga;

    public StartCreateOrderSagaCommandHandler(
        IOrderRepository repository,
        CreateOrderSaga createOrderSaga,
        ILogger<StartCreateOrderSagaCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
        _saga = createOrderSaga;
    }

    public async Task<CommandResult<Order>> Handle(StartCreateOrderSaga command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(StartCreateOrderSaga));

        try
        {
            // Start create order saga
            await _saga.StartSagaAsync(command.Order);
            
            // Return created order
            var order = await _repository.GetOrderAsync(command.EntityId);
            return order == null
                ? new CommandResult<Order>(CommandOutcome.NotFound)
                : new CommandResult<Order>(CommandOutcome.Accepted, order);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new CommandResult<Order>(CommandOutcome.InvalidState);
        }
    }
}