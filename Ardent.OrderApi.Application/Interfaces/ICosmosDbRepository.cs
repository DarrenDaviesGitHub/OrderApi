using Ardent.OrderApi.Domain.Models;

namespace Ardent.OrderApi.Application.Interfaces;

public interface ICosmosOrderRepository
{
    public Task CreateOrderAsync(Order order, CancellationToken cancellationToken);
    public Task<Order?> GetOrderAsync(Guid orderId, Guid customerId, CancellationToken cancellationToken);
}