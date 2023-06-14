using EventDriven.CQRS.Abstractions.Commands;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Persistence.Abstractions.Pools;
using OrderService.Domain.OrderAggregate.Commands;
using OrderService.Helpers;
using OrderService.Repositories;
using OrderService.Sagas.CreateOrder;

namespace OrderService.Domain.OrderAggregate.CommandHandlers;

public class StartCreateOrderSagaHandler : ICommandHandler<Order, StartCreateOrderSaga>
{
    private readonly IOrderRepository _repository;
    private readonly IPersistableSagaPool<CreateOrderSaga, OrderMetadata> _sagaPool;
    private readonly ILogger<StartCreateOrderSagaHandler> _logger;

    public StartCreateOrderSagaHandler(
        IOrderRepository repository,
        IPersistableSagaPool<CreateOrderSaga, OrderMetadata> sagaPool,
        ILogger<StartCreateOrderSagaHandler> logger)
    {
        _repository = repository;
        _sagaPool = sagaPool;
        _logger = logger;
    }

    public async Task<CommandResult<Order>> Handle(StartCreateOrderSaga command, CancellationToken cancellationToken)
    {
        if (command.Entity == null) return new CommandResult<Order>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);
        command.Entity.Apply(domainEvent);
        
        try
        {
            // When you emerge from here its the persistable saga has been saved
            var saga = await _sagaPool.CreateSagaAsync(command.OrderMetadata);
            
            // Start create order saga
            await saga.StartSagaAsync(command.Entity, cancellationToken);
            
            // Return created order
            var order = await _repository.GetAsync(command.Entity.Id);
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