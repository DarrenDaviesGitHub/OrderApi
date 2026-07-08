using Ardent.Domain.Models;
using Ardent.OrderApi.Application.DomainTransferObjects;
using AutoMapper;

namespace Ardent.OrderApi.Application.MappingProfiles;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        CreateMap<Product, ProductDto>();
    }
}