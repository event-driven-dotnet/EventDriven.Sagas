using AutoMapper;
using OrderService.DTO.Write;

namespace OrderService.DTO;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Order, OrderService.DTO.Write.Order>();
        CreateMap<Order, OrderService.DTO.Write.Order>().ReverseMap();
        CreateMap<OrderItem, OrderService.DTO.Write.OrderItem>();
        CreateMap<OrderItem, OrderService.DTO.Write.OrderItem>().ReverseMap();
        CreateMap<OrderState, OrderService.DTO.Write.OrderState>();
        CreateMap<OrderState, OrderService.DTO.Write.OrderState>().ReverseMap();
    }
}
