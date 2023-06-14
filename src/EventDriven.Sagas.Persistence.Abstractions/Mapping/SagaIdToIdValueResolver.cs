using AutoMapper;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions.Mapping;

/// <inheritdoc />
public class SagaIdToIdValueResolver<TMetadata> : 
    IValueResolver<PersistableSagaMetadataDto, PersistableSaga<TMetadata>, Guid>
    where TMetadata : class
{
    /// <inheritdoc />
    public Guid Resolve(PersistableSagaMetadataDto source, 
        PersistableSaga<TMetadata> destination, Guid destMember,
        ResolutionContext context) =>
        source.SagaId;
}