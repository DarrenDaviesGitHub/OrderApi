using Ardent.Domain.Models;

namespace Ardent.Infrastructure.Cosmos.Interfaces;

public interface ICosmosOrderRepository
{
    public Task CreateOrderAsync(Order order, CancellationToken cancellationToken);
    public Task<Order?> GetOrderAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken);
    // Update
    // Delete
}