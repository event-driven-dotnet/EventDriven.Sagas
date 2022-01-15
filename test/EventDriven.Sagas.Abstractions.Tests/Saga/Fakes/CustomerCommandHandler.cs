using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class CustomerCommandHandler : ISagaCommandHandler
{
    private readonly Customer _customer;
    private readonly ISagaCommandResultHandler<Customer> _resultProcessor;

    public CustomerCommandHandler(Customer customer, ISagaCommandResultHandler<Customer> resultProcessor)
    {
        _customer = customer;
        _resultProcessor = resultProcessor;
    }

    public async Task HandleAsync(ISagaCommand command, bool compensating)
    {
        if (command.Name!.EndsWith("Credit"))
        {
            var payload = ((FakeCommand)command).Result;
            _customer.Credit = payload;
            await _resultProcessor.HandleCommandResultAsync(_customer, compensating);
        }
    }
}