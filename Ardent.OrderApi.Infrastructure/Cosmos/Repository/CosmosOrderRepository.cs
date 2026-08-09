using Ardent.OrderApi.Domain.Models;
using Ardent.OrderApi.Infrastructure.Cosmos.Configuration;
using Ardent.OrderApi.Application.Interfaces;
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

    // Point Read
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

    // Query Read
    public async Task<IReadOnlyList<Order>> GetCustomerOrdersAsync(
        Guid customerId,
        CancellationToken cancellationToken)
    {
        List<Order> orders = [];

        var query = new QueryDefinition(
            """
            SELECT *
            FROM c
            WHERE c.customerId = @customerId
            """)
            .WithParameter("@customerId", customerId);

        var requestOptions = new QueryRequestOptions
        {
            PartitionKey = new PartitionKey(customerId.ToString())
        };

        using var iterator = _container.GetItemQueryIterator<OrderDocument>(
            query, 
            requestOptions: requestOptions);

        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync(cancellationToken);

            orders.AddRange(response.Select(OrderMapper.ToDomain));
        }

        return orders;
    }
}
