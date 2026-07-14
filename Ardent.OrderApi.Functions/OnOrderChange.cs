using Ardent.OrderApi.Shared.Cosmos.Documents;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Ardent.OrderApi.Functions;

public class OnOrderChange(ILogger<OnOrderChange> logger)
{
    [Function("OnOrderChange")]
    public void Run([CosmosDBTrigger(
        databaseName: "ardentdb",
        containerName: "orders",
        Connection = "CosmosDBConnection",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<OrderDocument> input)
        => ProcessChangedItems(input);

    private void ProcessChangedItems(IReadOnlyList<OrderDocument> orders)
    {
        if (orders is { Count: > 0 })
        {
            logger.LogInformation("Order documents modified: {Count}", orders.Count);

            foreach (var order in orders)
            {
                logger.LogInformation("Order Id: {DocumentId}", order.Id);
                logger.LogInformation("Customer Id: {CustomerId}", order.CustomerId);
                logger.LogInformation("Total Amount: {TotalAmount}", order.TotalAmount);
                logger.LogInformation("Order Date: {OrderDate}", order.OrderDate);
            }
        }
    }
}