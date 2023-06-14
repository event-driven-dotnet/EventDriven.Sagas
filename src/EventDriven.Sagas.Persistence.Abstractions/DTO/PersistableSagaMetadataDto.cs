namespace EventDriven.Sagas.Persistence.Abstractions.DTO;

/// <inheritdoc />
public class PersistableSagaMetadataDto : PersistableSagaDto
{
    /// <summary>
    /// Saga metadata.
    /// </summary>
    public string Metadata { get; set; } = null!;
}