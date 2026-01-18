using Ardent.Domain.Models;
using Ardent.Infrastructure.Cosmos.Interfaces;
using Ardent.OrderApi.Handlers;
using Ardent.OrderApi.Queries;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace Ardent.OrderApi.UnitTest.Handlers;

public class GetOrderQueryHandlerTests
{
    private readonly Mock<ICosmosOrderRepository> _orderRepositoryMock;
    private readonly GetOrderQueryHandler _handler;
    private readonly Fixture _fixture;

    public GetOrderQueryHandlerTests()
    {
        _orderRepositoryMock = new Mock<ICosmosOrderRepository>();
        _handler = new GetOrderQueryHandler(_orderRepositoryMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Handle_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        Guid orderId = _fixture.Create<Guid>();
        Guid customerId = _fixture.Create<Guid>();
        CancellationToken cancellationToken = CancellationToken.None;

        Order expectedOrder = _fixture.Build<Order>()
                                      .With(o => o.Id, orderId)
                                      .With(o => o.CustomerId, customerId)
                                      .Create();

        GetOrderQuery request = new(orderId, customerId);

        _orderRepositoryMock
            .Setup(r => r.GetOrderAsync(orderId, customerId, cancellationToken))
            .ReturnsAsync(expectedOrder);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(expectedOrder);

        _orderRepositoryMock.Verify(r => r.GetOrderAsync(orderId, customerId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnNull_WhenOrderDoesNotExist()
    {
        // Arrange
        Guid orderId = _fixture.Create<Guid>();
        Guid customerId = _fixture.Create<Guid>();
        CancellationToken cancellationToken = CancellationToken.None;

        GetOrderQuery request = new(orderId, customerId);

        _orderRepositoryMock
            .Setup(r => r.GetOrderAsync(orderId, customerId, cancellationToken))
            .ReturnsAsync((Order?)null);

        // Act
        var result = await _handler.Handle(request, cancellationToken);

        // Assert
        result.Should().BeNull();

        _orderRepositoryMock.Verify(r => r.GetOrderAsync(orderId, customerId, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Arrange
        Guid orderId = _fixture.Create<Guid>();
        Guid customerId = _fixture.Create<Guid>();
        CancellationToken cancellationToken = CancellationToken.None;
        GetOrderQuery? request = null;

        // Act
        var act = async () => await _handler.Handle(request!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();

        _orderRepositoryMock.Verify(r => r.GetOrderAsync(orderId, customerId, cancellationToken), Times.Never);
    }
}