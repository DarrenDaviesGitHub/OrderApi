using Ardent.Domain.Models;
using Ardent.Infrastructure.Cosmos.Interfaces;
using Ardent.OrderApi.Queries;
using MediatR;

namespace Ardent.OrderApi.Handlers;

public class GetOrderQueryHandler(ICosmosOrderRepository orderRepository) : IRequestHandler<GetOrderQuery, Order?>
{
    public async Task<Order?> Handle(
        GetOrderQuery request, 
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Console.WriteLine($"Handling GetOrdersQuery for OrderId: {request.OrderId} and CustomerId: {request.CustomerId}");
        
        return await orderRepository.GetOrderAsync(request.OrderId, request.CustomerId, cancellationToken);
    }
}
