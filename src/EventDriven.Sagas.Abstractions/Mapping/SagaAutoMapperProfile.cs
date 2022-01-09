using AutoMapper;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.DTO;
using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Mapping;

/// <summary>
///  Saga auto mapper configuration.
/// </summary>
public class SagaAutoMapperProfile: Profile
{
    /// <summary>
    /// Command type resolver.
    /// </summary>
    public static ISagaCommandTypeResolver? SagaCommandTypeResolver { get; set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    public SagaAutoMapperProfile()
    {
        CreateMap<SagaConfiguration, SagaConfigurationDto>();
        CreateMap<SagaConfiguration, SagaConfigurationDto>().ReverseMap();
        CreateMap<SagaStep, SagaStepDto>();
        CreateMap<SagaStep, SagaStepDto>().ReverseMap();
        CreateMap<SagaAction, SagaActionDto>();
        CreateMap<SagaAction, SagaActionDto>().ReverseMap();
        CreateMap<SagaCommand, string>()
            .ConvertUsing<SagaCommandToStringConverter>();
        CreateMap<string, SagaCommand>()
            .ConvertUsing(new SagaStringToCommandConverter(SagaCommandTypeResolver!));

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