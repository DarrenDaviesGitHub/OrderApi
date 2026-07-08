using Ardent.Domain.Models;
using Ardent.OrderApi.Application.DomainTransferObjects;
using AutoMapper;

namespace Ardent.OrderApi.Application.MappingProfiles;

public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<Order, OrderDto>();
    }
}