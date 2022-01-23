using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Domain.OrderAggregate;
using OrderService.Sagas.Commands;
using OrderService.Repositories;

namespace OrderService.Sagas.Handlers;

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
        _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStateCreated));
        
        try
        {
            // Set order state to created
            var updatedOrder = await _repository.UpdateOrderStateAsync(
                command.EntityId.GetValueOrDefault(), OrderState.Created);
            if (updatedOrder == null) return;

            await DispatchCommandResultAsync(updatedOrder.State, false);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
        }
    }
}