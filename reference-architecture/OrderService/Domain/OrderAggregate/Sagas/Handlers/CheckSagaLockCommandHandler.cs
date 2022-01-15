using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Handlers;
using OrderService.Domain.OrderAggregate.Sagas.Commands;
using OrderService.Repositories;

namespace OrderService.Domain.OrderAggregate.Sagas.Handlers;

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
        _logger.LogInformation("Handling command: {CommandName}", nameof(CreateOrder));
    
        var orderState = await _repository.GetOrderStateAsync(command.EntityId);
        return orderState is OrderState.Pending;
    }
}