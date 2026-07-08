using Ardent.OrderApi.Domain.Models;
using Ardent.OrderApi.Infrastructure.Cosmos.Configuration;
using Ardent.OrderApi.Infrastructure.Cosmos.Interfaces;
using Ardent.OrderApi.Infrastructure.Cosmos.Mappings;
using Ardent.OrderApi.Shared.Cosmos.Documents;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Ardent.OrderApi.Infrastructure.Cosmos.Repository;

public class CosmosOrderRepository : ICosmosOrderRepository
{
    private readonly Container _container;

    public CosmosOrderRepository(CosmosClient cosmosClient, IOptions<CosmosDbOptions> options)
    {
        var dbOptions = options.Value;

        _container = cosmosClient
            .GetContainer(dbOptions.DatabaseId, dbOptions.ContainerId);
    }

    public async Task CreateOrderAsync(Order order, CancellationToken cancellationToken)
    {
        var document = OrderMapper.ToDocument(order);

        await _container.CreateItemAsync(
            item: document,
            partitionKey: new PartitionKey(order.CustomerId.ToString()),
            cancellationToken: cancellationToken);
    }

    public async Task<Order?> GetOrderAsync(
        Guid orderId, 
        Guid customerId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _container.ReadItemAsync<OrderDocument>(
                id: orderId.ToString(),
                partitionKey: new PartitionKey(customerId.ToString()), 
                cancellationToken: cancellationToken);

            return OrderMapper.ToDomain(response.Resource);
        }
        catch (CosmosException ex)
            when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }
    }
}
