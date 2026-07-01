namespace Ardent.OrderApi.DomainTransferObjects;

public class ProductDto
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public decimal Price { get; init; }
}