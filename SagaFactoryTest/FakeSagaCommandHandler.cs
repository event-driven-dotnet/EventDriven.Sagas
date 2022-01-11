using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;

namespace SagaFactoryTest;

public class FakeSagaCommandHandler : SagaCommandHandler<FakeEntity, FakeSagaCommand>
{
    public override async Task<CommandResult<FakeEntity>> Handle(FakeSagaCommand command)
    {
        Console.WriteLine($"Handling saga command.");
        var result = new CommandResult<FakeEntity>(CommandOutcome.Accepted);

        await CommandResultProcessor?.ProcessCommandResultAsync(null!, false)!;
        return result;
    }
}