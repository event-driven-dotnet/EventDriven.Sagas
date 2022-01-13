using System.Threading.Tasks;
using EventDriven.DDD.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeSagaCommandHandler : 
    ResultDispatchingSagaCommandHandler<FakeSagaCommand, string>
{
    public override async Task<CommandResult> HandleCommandAsync(FakeSagaCommand command)
    {
        command.Result = "Success";
        await DispatchCommandResultAsync(command.Result, false);
        return new CommandResult(CommandOutcome.Accepted);
    }
}