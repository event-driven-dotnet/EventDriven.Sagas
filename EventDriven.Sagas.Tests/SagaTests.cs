using System.Threading.Tasks;
using EventDriven.Sagas.Tests.Fakes;
using Xunit;

namespace EventDriven.Sagas.Tests
{
    public class SagaTests
    {
        [Fact]
        public async Task SagaShouldComplete()
        {
            // Arrange
            var saga = new FakeCreateOrderSaga {Scenario = Scenario.Complete};

            // Act
            await saga.StartAsync();

            // Assert
        }
    }
}