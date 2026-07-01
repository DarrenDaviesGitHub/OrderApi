using Ardent.Domain.Models;
using Ardent.OrderApi.DomainTransferObjects;
using Ardent.OrderApi.MappingProfiles;
using Ardent.OrderApi.UnitTest.Helpers;
using AutoFixture;
using AutoMapper;
using FluentAssertions;

namespace Ardent.OrderApi.UnitTest.Mappings;

public class ProductProfileMappingTests
{
    private readonly IMapper _mapper;
    private readonly Fixture _fixture;

    public ProductProfileMappingTests()
    {
        var mapper = MapperFactory.Create(new ProductProfile());

        _mapper = mapper.Mapper;
        _fixture = new();
    }

    [Fact]
    public void Map_ProductToProductDto_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        var product = _fixture.Create<Product>();

        // Act
        var dto = _mapper.Map<ProductDto>(product);

        // Assert
        dto.Should()
           .BeEquivalentTo(product);

    }
}
