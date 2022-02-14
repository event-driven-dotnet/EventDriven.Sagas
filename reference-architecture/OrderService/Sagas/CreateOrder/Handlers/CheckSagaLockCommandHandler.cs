using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Domain.OrderAggregate;
using OrderService.Repositories;

namespace OrderService.Sagas.CreateOrder.Handlers;

public class CheckSagaLockCommandHandler : CheckSagaLockCommandHandler<CreateOrderSaga>
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<CheckSagaLockCommandHandler> _logger;

    public CheckSagaLockCommandHandler(
        IOrderRepository repository,
        ILogger<CheckSagaLockCommandHandler> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public override async Task<bool> HandleCommandAsync(CheckSagaLockCommand command)
    {
        _logger.LogInformation("Handling command: {CommandName}", nameof(Commands.CreateOrder));
    
        var orderState = await _repository.GetOrderStateAsync(command.EntityId.GetValueOrDefault());
        return orderState is OrderState.Pending;
    }
}