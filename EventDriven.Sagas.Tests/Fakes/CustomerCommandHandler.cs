using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Tests.Fakes;

public class CustomerCommandHandler : ISagaCommandHandler
{
    private readonly Customer _customer;
    private readonly ICommandResultProcessor<Customer> _resultProcessor;

    public CustomerCommandHandler(Customer customer, ICommandResultProcessor<Customer> resultProcessor)
    {
        _customer = customer;
        _resultProcessor = resultProcessor;
    }

    public async Task HandleAsync(ISagaCommand command, bool compensating)
    {
        if (command.Name!.EndsWith("Credit"))
        {
            var payload = ((FakeCommand)command).Payload;
            _customer.Credit = payload;
            await _resultProcessor.ProcessCommandResultAsync(_customer, compensating);
        }
    }
}