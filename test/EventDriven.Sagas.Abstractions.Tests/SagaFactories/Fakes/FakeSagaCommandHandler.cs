using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Handlers;

namespace EventDriven.Sagas.Abstractions.Tests.SagaFactories.Fakes;

public class FakeSagaCommandHandler<TSaga> : 
    ResultDispatchingSagaCommandHandler<TSaga, FakeSagaCommand, string>
    where TSaga : Abstractions.Saga
{
    public override async Task HandleCommandAsync(FakeSagaCommand command)
    {
        command.Result = "Success";
        await DispatchCommandResultAsync(command.Result, false, command.SagaId, command.Entity);
    }
}