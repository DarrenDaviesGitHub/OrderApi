using Newtonsoft.Json;

namespace Ardent.Infrastructure.Cosmos.Documents;

public class OrderDocument
{
    [JsonProperty(PropertyName="id")]
    public Guid Id { get; set; }

    [JsonProperty(PropertyName = "customerId")]
    public Guid CustomerId { get; set; }

    public List<ProductDocument> Products { get; set; } = new List<ProductDocument>();

    public decimal TotalAmount { get; set; }

    public DateTime OrderDate { get; set; }
}