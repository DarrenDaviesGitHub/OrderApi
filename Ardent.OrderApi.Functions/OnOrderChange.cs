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
        if (input is not null && input.Count > 0)
        {
            logger.LogInformation("Documents modified: " + input.Count);

            foreach (var inputItem in input)
            {
                logger.LogInformation($"Document Id: {inputItem.Id}");
                logger.LogInformation($"Customer Id: {inputItem.CustomerId}");
                logger.LogInformation($"Total Amount: {inputItem.TotalAmount}");
                logger.LogInformation($"Order Date: {inputItem.OrderDate}");
            }
        }
    }
}