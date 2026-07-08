using Ardent.OrderApi.Application.DomainTransferObjects;
using Ardent.OrderApi.Domain.Models;
using AutoMapper;

namespace Ardent.OrderApi.Application.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}