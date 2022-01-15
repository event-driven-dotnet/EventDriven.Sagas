using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Dispatchers;
using OrderService.Domain.OrderAggregate.Sagas.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Sagas.Handlers;

public class CreateOrderCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, CreateOrder, OrderState>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    public CreateOrderCommandHandler(
        IOrderRepository repository,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    // TODO: Refactor to return just Task
    public override async Task<CommandResult> HandleCommandAsync(CreateOrder command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
        
        try
        {
            // Add or update order
            if (command.Entity is not Order order)
                return new CommandResult(CommandOutcome.InvalidCommand);
            order.State = OrderState.Pending;
            var addedOrder = await _repository.AddUpdateOrderAsync(order);
            if (addedOrder == null) return new CommandResult<Order>(CommandOutcome.NotFound);

            await DispatchCommandResultAsync(order.State, false);
            return new CommandResult<Order>(CommandOutcome.Accepted, order);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new CommandResult<Order>(CommandOutcome.Conflict);
        }
    }
}