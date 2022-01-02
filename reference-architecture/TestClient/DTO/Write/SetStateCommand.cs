using EventDriven.Sagas.Abstractions.Commands;

namespace TestClient.DTO.Write;

public class SetStateCommand : ISagaCommand<OrderState, OrderState>
{
    public string? Name { get; set; }
    public OrderState Result { get; set; }
    public OrderState ExpectedResult { get; set; }
}