using AutoMapper;
using Entities = OrderService.Domain.OrderAggregate;
using Order = OrderService.DTO.Order;
using OrderItem = OrderService.DTO.OrderItem;

namespace OrderService.Mapping;

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
