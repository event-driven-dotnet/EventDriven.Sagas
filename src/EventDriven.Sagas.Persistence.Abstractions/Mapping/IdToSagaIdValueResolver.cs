using AutoMapper;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions.Mapping;

/// <inheritdoc />
public class IdToSagaIdValueResolver<TMetadata> : 
    IValueResolver<PersistableSaga<TMetadata>, PersistableSagaMetadataDto, Guid>
    where TMetadata : class
{
    /// <inheritdoc />
    public Guid Resolve(PersistableSaga<TMetadata> source, 
        PersistableSagaMetadataDto destination, Guid destMember,
        ResolutionContext context) =>
        source.Id;
}