using EventDriven.DDD.Abstractions.Commands;
using EventDriven.DDD.Abstractions.Repositories;
using EventDriven.Sagas.Abstractions.Commands;
using OrderService.Domain.OrderAggregate.Commands.SagaCommands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Commands.Handlers;

public class SetOrderStateCommandHandler :
    SagaCommandHandler<Order, SetOrderStatePending>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<SetOrderStateCommandHandler> _logger;

    public SetOrderStateCommandHandler(
        IOrderRepository repository,
        ILogger<SetOrderStateCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<CommandResult<Order>> Handle(SetOrderStatePending command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(SetOrderStatePending));
    
        var order = await _repository.GetOrderAsync(command.EntityId);
        if (order == null) return new CommandResult<Order>(CommandOutcome.NotFound);

        try
        {
            var updatedOrder = await _repository.UpdateOrderStateAsync(order, OrderState.Pending);
            if (updatedOrder == null)
                return new CommandResult<Order>(CommandOutcome.NotFound);
            if (CommandResultProcessor != null)
                await CommandResultProcessor.ProcessCommandResultAsync(updatedOrder, false);
            return new CommandResult<Order>(CommandOutcome.Accepted, updatedOrder);
        }
        catch (ConcurrencyException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new CommandResult<Order>(CommandOutcome.Conflict);
        }
    }
}