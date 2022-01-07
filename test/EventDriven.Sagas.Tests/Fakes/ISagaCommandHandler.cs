using System.Threading.Tasks;
using EventDriven.Sagas.Abstractions.Commands;

namespace EventDriven.Sagas.Tests.Fakes;

public interface ISagaCommandHandler
{
    Task HandleAsync(ISagaCommand command, bool compensating);
}