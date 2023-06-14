using System.Text.Json;
using AutoMapper;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions.Mapping;

/// <inheritdoc />
public class SagaMetadataToStringValueResolver<TMetadata> :
    IValueResolver<PersistableSaga<TMetadata>, PersistableSagaMetadataDto, string>
    where TMetadata : class
{
    /// <inheritdoc />
    public string Resolve(
        PersistableSaga<TMetadata> source, 
        PersistableSagaMetadataDto destination, 
        string destMember,
        ResolutionContext context) =>
        JsonSerializer.Serialize(source.Metadata, typeof(TMetadata));
}