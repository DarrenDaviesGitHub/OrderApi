namespace Ardent.OrderApi.DomainTransferObjects;

public class CreateOrderDto
{
    public Guid CustomerId { get; set; }
    public List<ProductDto> Products { get; set; } = [];
}