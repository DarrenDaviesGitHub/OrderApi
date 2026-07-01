using Ardent.Domain.Models;
using Ardent.OrderApi.DomainTransferObjects;
using Ardent.OrderApi.MappingProfiles;
using Ardent.OrderApi.UnitTest.Helpers;
using AutoFixture;
using AutoMapper;
using FluentAssertions;

namespace Ardent.OrderApi.UnitTest.Mappings;

public class OrderProfileMappingTests
{
    private readonly IMapper _mapper;
    private readonly Fixture _fixture;

    public OrderProfileMappingTests()
    {
        var mapper = MapperFactory.Create(
            new ProductProfile(), 
            new OrderProfile());

        _mapper = mapper.Mapper;
        _fixture = new();
    }

    [Fact]
    public void Map_OrderToOrderDto_ShouldMapPropertiesCorrectly()
    {
        // Arrange
        Order order = _fixture.Create<Order>();

        // Act
        var orderDto = _mapper.Map<OrderDto>(order);

        // Assert
        orderDto.Should()
                .NotBeNull();

        orderDto.Id.Should().Be(order.Id);
        orderDto.CustomerId.Should().Be(order.CustomerId);
        orderDto.TotalAmount.Should().Be(order.TotalAmount);
        orderDto.OrderDate.Should().Be(order.OrderDate);

        orderDto.Products.Should().NotBeNull();
    }
}
