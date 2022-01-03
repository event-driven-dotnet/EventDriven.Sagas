namespace TestClient.DTO;

public class SagaStep : EventDriven.Sagas.Abstractions.SagaStep
{
    public new SagaAction Action { get; set; } = null!;
    public new SagaAction CompensatingAction { get; set; } = null!;
}