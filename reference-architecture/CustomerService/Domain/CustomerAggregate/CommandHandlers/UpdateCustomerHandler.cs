using CustomerService.Domain.CustomerAggregate.Commands;
using CustomerService.Repositories;
using EventDriven.CQRS.Abstractions.Commands;

namespace CustomerService.Domain.CustomerAggregate.CommandHandlers;

public class UpdateCustomerHandler : ICommandHandler<Customer, UpdateCustomer>
{
    private readonly ICustomerRepository _repository;

    public UpdateCustomerHandler(
        ICustomerRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CommandResult<Customer>> Handle(UpdateCustomer command, CancellationToken cancellationToken)
    {
        // Process command
        if (command.Entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        var domainEvent = command.Entity.Process(command);
            
        // Apply events
        command.Entity.Apply(domainEvent);
            
        // Persist entity
        var entity = await _repository.UpdateAsync(command.Entity);
        if (entity == null) return new CommandResult<Customer>(CommandOutcome.InvalidCommand);
        return new CommandResult<Customer>(CommandOutcome.Accepted, entity);
    }
}