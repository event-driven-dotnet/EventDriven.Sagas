using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class RemoveCustomerHandler : ICommandHandler<RemoveCustomer>
{
    private readonly ICustomerRepository _repository;

    public RemoveCustomerHandler(
        ICustomerRepository repository)
    {
        _repository = repository;
    }

    public async Task<CommandResult> Handle(RemoveCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        var customer = await _repository.GetAsync(command.EntityId);
        if (customer == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = customer.Process(command);
        
        // Apply events
        customer.Apply(domainEvent);
        
        // Persist entity
        await _repository.RemoveAsync(command.EntityId);
        return new CommandResult(CommandOutcome.Accepted);
    }
}