namespace Ardent.OrderApi.Shared.Cosmos.Documents;

public class ProductDocument
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public decimal Price { get; set; }
}