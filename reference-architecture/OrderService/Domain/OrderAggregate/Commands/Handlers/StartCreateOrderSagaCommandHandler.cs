using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions;
using OrderService.Helpers;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

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
        var domainEvent = command.Entity.Process(command);
        command.Entity.Apply(domainEvent);
        
        try
        {
            // Start create order saga
            await _saga.StartSagaAsync(command.Entity);
            
            // Return created order
            var order = await _repository.GetAsync(command.EntityId);
            return order == null
                ? new CommandResult<Order>(CommandOutcome.NotFound)
                : new CommandResult<Order>(CommandOutcome.Accepted, order);
        }
        catch (SagaLockedException e)
        {
            _logger.LogError(e, "{Message}", e.Message);
            return new CommandResult<Order>(CommandOutcome.Conflict, e.ToErrors());
        }
    }
}