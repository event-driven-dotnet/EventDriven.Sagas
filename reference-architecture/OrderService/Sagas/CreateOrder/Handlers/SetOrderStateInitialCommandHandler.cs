using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder.Commands;

namespace OrderService.Sagas.CreateOrder.Handlers;

public class SetOrderStateInitialCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, SetOrderStateInitial, OrderState>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<SetOrderStateInitialCommandHandler> _logger;

    public SetOrderStateInitialCommandHandler(
        IOrderRepository repository,
        ILogger<SetOrderStateInitialCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public override async Task HandleCommandAsync(SetOrderStateInitial command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStateInitial));
        
        try
        {
            // Set order state to initial
            var updatedOrder = await _repository.UpdateOrderStateAsync(
                command.EntityId.GetValueOrDefault(), OrderState.Initial);
            if (updatedOrder != null)
                await DispatchCommandResultAsync(updatedOrder.State, true);
            else
                await DispatchCommandResultAsync(OrderState.Pending, true);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            await DispatchCommandResultAsync(OrderState.Pending, true);
        }
    }
}