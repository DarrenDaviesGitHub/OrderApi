namespace Ardent.OrderApi.Application.DomainTransferObjects;

public class CreateOrderDto
{
    public Guid CustomerId { get; init; }
    public List<ProductDto> Products { get; init; } = [];
}