using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Abstractions.Tests.Saga.Fakes;

public interface ISagaCommandHandler
{
    Task HandleAsync(ISagaCommand command, bool compensating);
}