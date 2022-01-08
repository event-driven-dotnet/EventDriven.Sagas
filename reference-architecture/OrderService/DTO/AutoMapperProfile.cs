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
    }
}
