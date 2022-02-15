using AutoMapper;
using InventoryService.DTO;
using Entities = InventoryService.Domain.InventoryAggregate;

namespace InventoryService.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Entities.Inventory, Inventory>();
        CreateMap<Entities.Inventory, Inventory>().ReverseMap();
    }
}