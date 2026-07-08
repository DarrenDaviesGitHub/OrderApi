using Ardent.OrderApi.Application.DomainTransferObjects;
using Ardent.OrderApi.Domain.Models;
using AutoMapper;

namespace Ardent.OrderApi.Application.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}