namespace Ardent.OrderApi.Domain.Models;

public record Product(Guid Id, string Name, string Description, decimal Price);