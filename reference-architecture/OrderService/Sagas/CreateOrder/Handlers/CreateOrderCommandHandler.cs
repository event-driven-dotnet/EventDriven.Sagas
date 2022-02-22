using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;

namespace OrderService.Sagas.CreateOrder.Handlers;

public class CreateOrderCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, Commands.CreateOrder, OrderState>
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
    
    public override async Task HandleCommandAsync(Commands.CreateOrder command)
    {
        try
        {
            // Add or update order
            _logger.LogInformation("Handling command: {CommandName}", nameof(Commands.CreateOrder));
            if (command.Entity is not Order order) return;
            order.State = OrderState.Pending;
            var addedOrder = await _repository.AddUpdateAsync(order);
            if (addedOrder != null)
                await DispatchCommandResultAsync(addedOrder.State, false);
            else
                await DispatchCommandResultAsync(OrderState.Initial, true);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            await DispatchCommandResultAsync(OrderState.Initial, true);
        }
    }
}