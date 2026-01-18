using Ardent.Domain.Models;
using Ardent.Infrastructure.Cosmos.Documents;

namespace Ardent.Infrastructure.Cosmos.Mappings;

public class OrderMapper
{
    public static OrderDocument ToDocument(Order order)
        => new()
        {
            Id = order.Id,
            CustomerId = order.CustomerId,
            Products = order.Products
                .Select(p => new ProductDocument
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Price = p.Price
                })
                .ToList(),
            TotalAmount = order.TotalAmount,
            OrderDate = order.OrderDate
        };

    public static Order ToDomain(OrderDocument doc)
        => new(
            doc.Id,
            doc.CustomerId,
            doc.Products.Select(p => new Product(p.Id, p.Name, p.Description, p.Price)).ToList(),
            doc.TotalAmount,
            doc.OrderDate
        );
}