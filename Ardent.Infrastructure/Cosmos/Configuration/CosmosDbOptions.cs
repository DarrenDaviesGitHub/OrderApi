namespace Ardent.OrderApi.Infrastructure.Cosmos.Configuration;

public class CosmosDbOptions
{
    public string AccountEndpoint { get; set; } = null!;
    public string AccountKey { get; set; } = null!;
    public string DatabaseId { get; set; } = null!;
    public string ContainerId { get; set; } = null!;
}