using AutoMapper;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions.Mapping;

/// <inheritdoc />
public class StartedToSagaStartedValueResolver<TMetadata> :
    IValueResolver<PersistableSaga<TMetadata>, PersistableSagaMetadataDto, DateTime>
    where TMetadata : class
{
    /// <inheritdoc />
    public DateTime Resolve(PersistableSaga<TMetadata> source, 
        PersistableSagaMetadataDto destination, DateTime destMember,
        ResolutionContext context) =>
        source.Started;
}