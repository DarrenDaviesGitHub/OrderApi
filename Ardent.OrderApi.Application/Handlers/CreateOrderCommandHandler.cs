using Ardent.OrderApi.Application.Commands;
using Ardent.OrderApi.Domain.Models;
using Ardent.OrderApi.Infrastructure.Cosmos.Interfaces;
using MediatR;

namespace Ardent.OrderApi.Application.Handlers;

public class CreateOrderCommandHandler(
    ICosmosOrderRepository cosmosOrderRepository)
    : IRequestHandler<CreateOrderCommand>
{
    public async Task Handle(
        CreateOrderCommand command,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(command);

        Guid orderId = Guid.NewGuid();
        Order domainOrder = CreateOrder(command, orderId);

        Console.WriteLine($"Attempting to create a new order for customer: {command.order.CustomerId}, Order Id: {orderId}");

        await cosmosOrderRepository.CreateOrderAsync(domainOrder, cancellationToken);
    }

    private static Order CreateOrder(CreateOrderCommand command, Guid orderId)
        => new(orderId,
           command.order.CustomerId,
           command.order.Products.Select(item => new Product(Guid.NewGuid(), item.Name, item.Description, item.Price)).ToList(),
           command.order.Products.Sum(p => p.Price),
           DateTime.UtcNow);
}