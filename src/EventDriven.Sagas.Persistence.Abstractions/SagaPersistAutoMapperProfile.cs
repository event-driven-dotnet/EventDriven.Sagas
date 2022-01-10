using AutoMapper;
using EventDriven.Sagas.Abstractions.Mapping;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions;

/// <summary>
///  Saga auto mapper configuration.
/// </summary>
public class SagaConfigAutoMapperProfile: Profile
{
    /// <summary>
    /// Command type resolver.
    /// </summary>
    public static ISagaCommandTypeResolver? SagaCommandTypeResolver { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SagaConfigAutoMapperProfile()
    {
        CreateMap<PersistableSaga, SagaSnapshotDto>()
            .ForMember(dest => dest.SagaId, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SagaStarted, opt =>
                opt.MapFrom(src => src.Started));
        CreateMap<SagaSnapshotDto, PersistableSaga>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.SagaId))
            .ForMember(dest => dest.Started, opt =>
                opt.MapFrom(src => src.SagaStarted));
    }
}