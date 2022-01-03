using AutoMapper;
using OrderService.DTO.Write;

namespace OrderService.DTO;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Order, Order>();
        CreateMap<Order, Order>().ReverseMap();
        CreateMap<OrderItem, OrderItem>();
        CreateMap<OrderItem, OrderItem>().ReverseMap();
        CreateMap<OrderState, OrderState>();
        CreateMap<OrderState, OrderState>().ReverseMap();
    }
}
