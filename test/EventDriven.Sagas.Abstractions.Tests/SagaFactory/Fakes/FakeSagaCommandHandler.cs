using System.Threading.Tasks;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeSagaCommandHandler : SagaCommandHandler<FakeEntity, FakeSagaCommand>
{
    public override async Task<CommandResult<FakeEntity>> Handle(FakeSagaCommand command)
    {
        command.Result = "Success";
        var entity = new FakeEntity { State = command.Result };
        if (CommandResultProcessor != null)
            await CommandResultProcessor.ProcessCommandResultAsync(entity, false);
        return new CommandResult<FakeEntity>(CommandOutcome.Accepted);
    }
}