using AutoMapper;
using EventDriven.Sagas.Abstractions;
using EventDriven.Sagas.Abstractions.Repositories;

namespace TestClient.Serialization;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<SagaConfiguration, SerializableSagaConfiguration>();
        CreateMap<SagaConfiguration, SerializableSagaConfiguration>().ReverseMap();
        CreateMap<SagaStep, SerializableSagaStep>();
        CreateMap<SagaStep, SerializableSagaStep>().ReverseMap();
        CreateMap<SagaAction, SerializableSagaAction>();
        CreateMap<SagaAction, SerializableSagaAction>().ReverseMap();
    }
}
