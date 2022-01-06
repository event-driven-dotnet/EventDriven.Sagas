using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Tests.Fakes;

public class InMemoryCommandDispatcher : ISagaCommandDispatcher
{
    public OrderCommandHandler OrderCommandHandler { get; set; } = null!;
    public CustomerCommandHandler CustomerCommandHandler { get; set; } = null!;
    public InventoryCommandHandler InventoryCommandHandler { get; set; } = null!;

    public async Task DispatchAsync(SagaCommand command, bool compensating)
    {
        ISagaCommandHandler? handler = null;
        if (command.Name!.StartsWith("SetState"))
            handler = OrderCommandHandler;
        else if (command.Name!.EndsWith("Credit"))
            handler = CustomerCommandHandler;
        else if (command.Name!.EndsWith("Inventory"))
            handler = InventoryCommandHandler;
        if (handler != null)
            await DispatchCommandAsync(handler, command, compensating);
    }

    private async Task DispatchCommandAsync(ISagaCommandHandler commandHandler, ISagaCommand command, bool compensating)
        => await commandHandler.HandleAsync(command, compensating);
}