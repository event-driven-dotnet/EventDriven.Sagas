using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder.Commands;

namespace OrderService.Sagas.CreateOrder.Handlers;

public class SetOrderStateCreatedCommandHandler :
    ResultDispatchingSagaCommandHandler<CreateOrderSaga, SetOrderStateCreated, OrderState>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<SetOrderStateCreatedCommandHandler> _logger;

    public SetOrderStateCreatedCommandHandler(
        IOrderRepository repository,
        ILogger<SetOrderStateCreatedCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }
    
    public override async Task HandleCommandAsync(SetOrderStateCreated command)
    {
        try
        {
            // Set order state to created
            _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStateCreated));
            var updatedOrder = await _repository.UpdateOrderStateAsync(
                command.EntityId.GetValueOrDefault(), OrderState.Created);
            if (updatedOrder != null)
                await DispatchCommandResultAsync(updatedOrder.State, false);
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