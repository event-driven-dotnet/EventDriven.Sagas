using AutoMapper;
using Entities = OrderService.Domain.OrderAggregate;
using SagaEntities = EventDriven.Sagas.Abstractions;

namespace OrderService.DTO;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Entities.Order, Order>();
        CreateMap<Entities.Order, Order>().ReverseMap();
        CreateMap<Entities.OrderItem, OrderItem>();
        CreateMap<Entities.OrderItem, OrderItem>().ReverseMap();
        // CreateMap<Entities.OrderState, OrderState>();
        // CreateMap<Entities.OrderState, OrderState>().ReverseMap();
        
        // CreateMap<SagaEntities.SagaConfiguration, SagaConfiguration>()
        //     .ForMember(dest => dest.Steps,
        //         source => source.MapFrom());
        // CreateMap<SagaEntities.SagaConfiguration, SagaConfiguration>().ReverseMap();
        // CreateMap<SagaEntities.SagaStep, SagaEntities.SagaStep>();
        // CreateMap<SagaEntities.SagaStep, SagaEntities.SagaStep>().ReverseMap();
        // CreateMap<SagaEntities.SagaAction, SagaEntities.SagaAction>();
        // CreateMap<SagaEntities.SagaAction, SagaEntities.SagaAction>().ReverseMap();
    }
}
