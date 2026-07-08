using Ardent.Domain.Models;
using Ardent.Infrastructure.Cosmos.Interfaces;
using Ardent.OrderApi.Application.Commands;
using Ardent.OrderApi.Application.DomainTransferObjects;
using Ardent.OrderApi.Application.Handlers;
using AutoFixture;
using FluentAssertions;
using Moq;

namespace Ardent.OrderApi.Application.UnitTest.Handlers;

public class CreateOrderCommandHandlerTests
{
    private readonly Fixture _fixture;
    private readonly Mock<ICosmosOrderRepository> _repositoryMock;
    private readonly CreateOrderCommandHandler _handler;

    public CreateOrderCommandHandlerTests()
    {
        _fixture = new Fixture();
        _repositoryMock = new Mock<ICosmosOrderRepository>();
        _handler = new CreateOrderCommandHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldThrowArgumentNullException_WhenCommandIsNull()
    {
        // Arrange
        CreateOrderCommand? command = null;

        // Act
        var act = async () => await _handler.Handle(command!, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>();
        _repositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldCreateOrder_WhenCommandIsValid()
    {
        // Arrange
        Guid customerId = _fixture.Create<Guid>();
        CancellationToken cancellationToken = CancellationToken.None;
        List<ProductDto> productDtos = [.. _fixture.CreateMany<ProductDto>(3)];
        CreateOrderDto order = _fixture.Build<CreateOrderDto>()
                                       .With(o => o.CustomerId, customerId)
                                       .With(o => o.Products, productDtos)
                                       .Create();

        CreateOrderCommand command = _fixture.Build<CreateOrderCommand>()
                                             .With(c => c.order, order)
                                             .Create();

        _repositoryMock.Setup(r => r.CreateOrderAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
                       .Returns(Task.CompletedTask);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _repositoryMock.Verify(r => r.CreateOrderAsync(It.IsAny<Order>(), CancellationToken.None), Times.Once);
    }
}
