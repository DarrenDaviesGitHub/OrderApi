using Ardent.Domain.Models;
using Ardent.OrderApi.Commands;
using Ardent.OrderApi.Controllers;
using Ardent.OrderApi.DomainTransferObjects;
using Ardent.OrderApi.Queries;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Ardent.OrderApi.UnitTest.Controllers;

public class OrderControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly OrderController _controller;
    private readonly Fixture _fixture;

    public OrderControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new OrderController(_mediatorMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task Get_ShouldReturnOrder_WhenOrderExists()
    {
        // Arrange
        Guid orderId = _fixture.Create<Guid>();
        Guid customerId = _fixture.Create<Guid>();
        CancellationToken cancellationToken = CancellationToken.None;

        Order expectedOrder = _fixture.Build<Order>()
                                      .With(o => o.Id, orderId)
                                      .With(o => o.CustomerId, customerId)
                                      .With(o => o.Products, [.. _fixture.CreateMany<Product>(2)])
                                      .Create();

        GetOrderQuery getOrderQuery = _fixture.Build<GetOrderQuery>()
                                              .With(q => q.OrderId, orderId)
                                              .With(q => q.CustomerId, customerId)
                                              .Create();

        _mediatorMock.Setup(m => m.Send(getOrderQuery, cancellationToken)).ReturnsAsync(expectedOrder);

        // Act
        var result = await _controller.Get(orderId, customerId);

        // Assert
        result.Result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();

        var orderResult = (result.Result as OkObjectResult)!.Value as Order;

        orderResult.Should().NotBeNull();
        orderResult.Should().BeEquivalentTo(expectedOrder);
        orderResult.Id.Should().Be(orderId);
        orderResult.CustomerId.Should().Be(customerId);

        _mediatorMock.Verify(m => m.Send(getOrderQuery, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        Guid orderId = _fixture.Create<Guid>();
        Guid customerId = _fixture.Create<Guid>();
        CancellationToken cancellationToken = CancellationToken.None;

        GetOrderQuery getOrderQuery = _fixture.Build<GetOrderQuery>()
                                              .With(q => q.OrderId, orderId)
                                              .With(q => q.CustomerId, customerId)
                                              .Create();

        _mediatorMock.Setup(m => m.Send<Order?>(getOrderQuery, cancellationToken))
                     .ReturnsAsync((Order?)null);

        // Act
        var result = await _controller.Get(orderId, customerId);

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
        result.Value.Should().BeNull();

        _mediatorMock.Verify(m => m.Send(getOrderQuery, cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Post_ShouldReturn201Created_WhenOrderIsValid()
    {
        // Arrange
        CreateOrderDto orderDto = _fixture.Create<CreateOrderDto>();
        CreateOrderCommand createOrderCommand = _fixture.Build<CreateOrderCommand>()
                                                        .With(c => c.order, orderDto)
                                                        .Create();
        CancellationToken cancellationToken = CancellationToken.None;

        _mediatorMock.Setup(m => m.Send(createOrderCommand, cancellationToken)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Post(orderDto);

        // Assert
        result.Should().BeOfType<StatusCodeResult>();
        (result as StatusCodeResult)!.StatusCode.Should().Be(StatusCodes.Status201Created);

        _mediatorMock.Verify(m => m.Send(It.Is<CreateOrderCommand>(c => c.order == orderDto), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Post_ShouldThrowException_WhenServerSideErrorOccurs()
    {
        // Arrange
        CreateOrderDto orderDto = _fixture.Create<CreateOrderDto>();
        CreateOrderCommand createOrderCommand = _fixture.Build<CreateOrderCommand>()
                                                        .With(c => c.order, orderDto)
                                                        .Create();
        CancellationToken cancellationToken = CancellationToken.None;

        _mediatorMock.Setup(m => m.Send(createOrderCommand, cancellationToken))
                     .ThrowsAsync(new Exception("Server Side Error"));

        // Act
        var result = async () => await _controller.Post(orderDto);

        // Assert
        await result.Should().ThrowAsync<Exception>();
    }
}
