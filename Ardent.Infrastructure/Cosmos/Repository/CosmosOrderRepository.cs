using Ardent.Domain.Models;
using Ardent.Infrastructure.Cosmos.Configuration;
using Ardent.Infrastructure.Cosmos.Documents;
using Ardent.Infrastructure.Cosmos.Interfaces;
using Ardent.Infrastructure.Cosmos.Mappings;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Options;

namespace Ardent.Infrastructure.Cosmos.Repository;

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
