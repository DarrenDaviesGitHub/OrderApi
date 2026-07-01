using Ardent.Domain.Models;
using Ardent.OrderApi.DomainTransferObjects;
using AutoMapper;

namespace Ardent.OrderApi.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}