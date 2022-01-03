namespace TestClient.DTO;

public class SagaAction : EventDriven.Sagas.Abstractions.SagaAction
{
    public new string Command { get; set; } = null!;
}