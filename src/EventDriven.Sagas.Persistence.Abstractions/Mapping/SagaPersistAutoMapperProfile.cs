using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Mapping;
using EventDriven.Sagas.Persistence.Abstractions.DTO;

namespace EventDriven.Sagas.Persistence.Abstractions.Mapping;

/// <summary>
///  Saga auto mapper configuration.
/// </summary>
public class SagaPersistAutoMapperProfile: Profile
{
    /// <summary>
    /// Command type resolver.
    /// </summary>
    public static ISagaCommandTypeResolver? SagaCommandTypeResolver { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SagaPersistAutoMapperProfile()
    {
        CreateMap<SagaStep, SagaStepDto>();
        CreateMap<SagaStep, SagaStepDto>().ReverseMap();
        CreateMap<SagaAction, SagaActionDto>();
        CreateMap<SagaAction, SagaActionDto>().ReverseMap();
        CreateMap<SagaCommand, string>()
            .ConvertUsing(new SagaCommandToStringConverter());
        CreateMap<string, SagaCommand>()
            .ConvertUsing(new SagaStringToCommandConverter(SagaCommandTypeResolver!));
        CreateMap<PersistableSaga, SagaSnapshotDto>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => Guid.Empty))
            .ForMember(dest => dest.SagaId, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SagaStarted, opt =>
                opt.MapFrom(src => src.Started));
        CreateMap<SagaSnapshotDto, PersistableSaga>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => Guid.Empty))
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.SagaId))
            .ForMember(dest => dest.Started, opt =>
                opt.MapFrom(src => src.SagaStarted));
        CreateMap<PersistableSaga, PersistableSagaDto>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => Guid.Empty))
            .ForMember(dest => dest.SagaId, opt =>
                opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.SagaStarted, opt =>
                opt.MapFrom(src => src.Started));
        CreateMap<PersistableSagaDto, PersistableSaga>()
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => Guid.Empty))
            .ForMember(dest => dest.Id, opt =>
                opt.MapFrom(src => src.SagaId))
            .ForMember(dest => dest.Started, opt =>
                opt.MapFrom(src => src.SagaStarted));
        CreateMap(typeof(PersistableSaga<>), typeof(PersistableSagaMetadataDto))
            .ForMember("Id", opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember("SagaId", opt => opt.MapFrom(typeof(IdToSagaIdValueResolver<>)))
            .ForMember("SagaStarted", opt => opt.MapFrom(typeof(StartedToSagaStartedValueResolver<>)))
            .ForMember("Metadata", opt => opt.MapFrom(typeof(SagaMetadataToStringValueResolver<>)));
        CreateMap(typeof(PersistableSagaMetadataDto), typeof(PersistableSaga<>))
            .ForMember("Id", opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember("Id", opt => opt.MapFrom(typeof(SagaIdToIdValueResolver<>)))
            .ForMember("Started", opt => opt.MapFrom(typeof(SagaStartedToStartedValueResolver<>)))
            .ForMember("Metadata", opt => opt.Ignore());
    }
}
