using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactory.Fakes;

public class FakeSagaCommandHandler : 
    ResultDispatchingSagaCommandHandler<FakeSagaCommand, string>
{
    public override async Task HandleCommandAsync(FakeSagaCommand command)
    {
        command.Result = "Success";
        await DispatchCommandResultAsync(command.Result, false);
    }
}