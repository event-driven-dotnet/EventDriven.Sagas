using AutoMapper;
using EventDriven.Sagas.Abstractions.Commands;
using EventDriven.Sagas.Abstractions.Entities;

namespace EventDriven.Sagas.Abstractions.Mapping;

/// <summary>
///  Saga auto mapper configuration.
/// </summary>
public class SagaAutoMapperProfile : Profile
{
    /// <summary>
    /// SagaAutoMapperProfile constructor.
    /// </summary>
    public SagaAutoMapperProfile()
    {
        CreateMap<SagaConfiguration, DTO.SagaConfigurationDto>();
        CreateMap<SagaConfiguration, DTO.SagaConfigurationDto>().ReverseMap();
        CreateMap<SagaStep, DTO.SagaStepDto>();
        CreateMap<SagaStep, DTO.SagaStepDto>().ReverseMap();
        CreateMap<SagaAction, DTO.SagaActionDto>();
        CreateMap<SagaAction, DTO.SagaActionDto>().ReverseMap();

        CreateMap<SagaCommand, string>()
            .ConvertUsing<SagaCommandToStringConverter>();
        CreateMap<string, SagaCommand>()
            .ConvertUsing(new SagaStringToCommandConverter(new SagaCommandTypeResolver()));
    }
}