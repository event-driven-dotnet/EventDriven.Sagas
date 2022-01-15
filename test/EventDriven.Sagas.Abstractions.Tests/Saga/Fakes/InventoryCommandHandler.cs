using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class InventoryCommandHandler : ISagaCommandHandler
{
    private readonly Inventory _inventory;
    private readonly ISagaCommandResultHandler<Inventory> _resultProcessor;

    public InventoryCommandHandler(Inventory inventory,
        ISagaCommandResultHandler<Inventory> resultProcessor)
    {
        _inventory = inventory;
        _resultProcessor = resultProcessor;
    }

    public async Task HandleAsync(ISagaCommand command, bool compensating)
    {
        if (command.Name!.EndsWith("Inventory"))
        {
            var payload = ((FakeCommand)command).Result;
            _inventory.Stock = payload;
            await _resultProcessor.HandleCommandResultAsync(_inventory, compensating);
        }
    }
}