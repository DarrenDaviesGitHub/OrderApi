using Ardent.Domain.Models;
using Ardent.OrderApi.DomainTransferObjects;
using AutoMapper;

namespace Ardent.OrderApi.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}