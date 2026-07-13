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

    private void ProcessChangedItems(IReadOnlyList<OrderDocument> input)
    {
        if (input is { Count: > 0 })
        {
            logger.LogInformation("Documents modified: {Count}", input.Count);

            foreach (var inputItem in input)
            {
                logger.LogInformation("Document Id: {DocumentId}", inputItem.Id);
                logger.LogInformation("Customer Id: {CustomerId}", inputItem.CustomerId);
                logger.LogInformation("Total Amount: {TotalAmount}", inputItem.TotalAmount);
                logger.LogInformation("Order Date: {OrderDate}", inputItem.OrderDate);
            }
        }
    }
}