namespace Ardent.Infrastructure.Cosmos.Documents;

public class ProductDocument
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
}