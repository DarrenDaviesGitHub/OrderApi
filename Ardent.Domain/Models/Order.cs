namespace Ardent.Domain.Models;

public record Order(Guid Id, Guid CustomerId, List<Product> Products, decimal TotalAmount, DateTime OrderDate);
