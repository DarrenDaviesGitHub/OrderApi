namespace Ardent.OrderApi.Application.DomainTransferObjects;

public class OrderDto
{
    public Guid Id { get; init; }
    public Guid CustomerId { get; init; }
    public List<ProductDto> Products { get; init; } = [];
    public decimal TotalAmount { get; init; }
    public DateTime OrderDate { get; init; }
}
