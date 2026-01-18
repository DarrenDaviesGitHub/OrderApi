using Ardent.Domain.Models;
using Ardent.Infrastructure.Cosmos.Documents;
using Ardent.Infrastructure.Cosmos.Mappings;
using AutoFixture;
using FluentAssertions;

namespace Ardent.Infrastructure.UnitTest.Cosmos.Mappings;

public class OrderMapperTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void ToDomain_ShouldMapOrderDocument_ToOrder()
    {
        // Arrange
        Guid orderId = _fixture.Create<Guid>();
        Guid customerId = _fixture.Create<Guid>();
        Guid productId = _fixture.Create<Guid>();
        OrderDocument orderDocument = _fixture.Build<OrderDocument>()
                                    .With(d => d.Id, orderId)
                                    .With(d => d.CustomerId, customerId)
                                    .With(d => d.Products, [_fixture.Build<ProductDocument>().With(pd => pd.Id, productId).Create()])
                                    .Create();

        // Act
        var order = OrderMapper.ToDomain(orderDocument);

        // Assert
        order.Should().NotBeNull();
        order.Id.Should().Be(orderDocument.Id);
        order.CustomerId.Should().Be(orderDocument.CustomerId);
        order.TotalAmount.Should().Be(orderDocument.TotalAmount);
        order.OrderDate.Should().Be(orderDocument.OrderDate);
    }

    [Fact]
    public void ToDocument_ShouldMapOrder_ToOrderDocument()
    {
        // Arrange
        Order order = _fixture.Build<Order>()
                            .With(o => o.Products, [.. _fixture.CreateMany<Product>(3)])
                            .Create();

        // Act
        var doc = OrderMapper.ToDocument(order);

        // Assert
        doc.Should().NotBeNull();
        doc.Id.Should().Be(order.Id.ToString());
        doc.CustomerId.Should().Be(order.CustomerId);
        doc.TotalAmount.Should().Be(order.TotalAmount);
        doc.OrderDate.Should().Be(order.OrderDate);
    }
}
