using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Mapping;
using EventDriven.Sagas.Configuration.Abstractions.DTO;

namespace EventDriven.Sagas.Configuration.Abstractions;

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
        CreateMap<SagaConfiguration, SagaConfigurationDto>();
        CreateMap<SagaConfiguration, SagaConfigurationDto>().ReverseMap();
        CreateMap<SagaStep, SagaStepDto>();
        CreateMap<SagaStep, SagaStepDto>().ReverseMap();
        CreateMap<SagaAction, SagaActionDto>();
        CreateMap<SagaAction, SagaActionDto>().ReverseMap();
        CreateMap<SagaCommand, string>()
            .ConvertUsing(new SagaCommandToStringConverter());
        CreateMap<string, SagaCommand>()
            .ConvertUsing(new SagaStringToCommandConverter(SagaCommandTypeResolver!));
    }
}