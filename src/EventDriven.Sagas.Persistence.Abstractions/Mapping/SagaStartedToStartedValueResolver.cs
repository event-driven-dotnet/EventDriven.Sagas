using AutoMapper;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions.Mapping;

/// <inheritdoc />
public class SagaStartedToStartedValueResolver<TMetadata> : 
    IValueResolver<PersistableSagaMetadataDto, PersistableSaga<TMetadata>, DateTime>
    where TMetadata : class
{
    /// <inheritdoc />
    public DateTime Resolve(PersistableSagaMetadataDto source, 
        PersistableSaga<TMetadata> destination, DateTime destMember,
        ResolutionContext context) =>
        source.SagaStarted;
}