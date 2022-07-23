using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.Sagas.Abstractions;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Helpers;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

public class StartCreateOrderSagaHandler : ICommandHandler<Order, StartCreateOrderSaga>
{
    private readonly IOrderRepository _repository;
    private readonly CreateOrderSaga _createOrderSaga;
    private readonly ILogger<StartCreateOrderSagaHandler> _logger;

    public StartCreateOrderSagaHandler(
        IOrderRepository repository,
        CreateOrderSaga createOrderSaga,
        ILogger<StartCreateOrderSagaHandler> logger)
    {
        _repository = repository;
        _createOrderSaga = createOrderSaga;
        _logger = logger;
    }

    public async Task<CommandResult<Order>> Handle(StartCreateOrderSaga command, CancellationToken cancellationToken)
    {
        if (command.Entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);
        command.Entity.Apply(domainEvent);
        
        try
        {
            // Start create order saga
            await _createOrderSaga.StartSagaAsync(command.Entity, command.OrderMetadata, cancellationToken);
            
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