﻿using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public class InventoryCommandHandler : ISagaCommandHandler
{
    private readonly Inventory _inventory;
    private readonly ICommandResultProcessor<Inventory> _resultProcessor;

    public InventoryCommandHandler(Inventory inventory,
        ICommandResultProcessor<Inventory> resultProcessor)
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
            await _resultProcessor.ProcessCommandResultAsync(_inventory, compensating);
        }
    }
}