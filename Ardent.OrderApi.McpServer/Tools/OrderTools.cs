using System.ComponentModel;
using Ardent.OrderApi.Application.Commands;
using Ardent.OrderApi.Application.DomainTransferObjects;
using Ardent.OrderApi.Application.Queries;
using MediatR;
using ModelContextProtocol;
using ModelContextProtocol.Server;

namespace Ardent.OrderApi.McpServer.Tools;

[McpServerToolType]
public class OrderTools(IMediator mediator)
{
    [McpServerTool(Name = "get_order", Title = "Get order", ReadOnly = true)]
    [Description("Retrieves a single order for a customer. Both the order id and the customer id are required because orders are partitioned by customer.")]
    public async Task<OrderDto> GetOrderAsync(
        [Description("GUID identifier of the order to retrieve")] Guid orderId,
        [Description("GUID identifier of the customer that owns the order")] Guid customerId,
        CancellationToken cancellationToken)
    {
        var order = await mediator.Send(new GetOrderQuery(orderId, customerId), cancellationToken);

        return order ?? throw new McpException($"No order was found with id {orderId} for customer {customerId}.");
    }

    [McpServerTool(Name = "get_customer_orders", Title = "Get customer orders", ReadOnly = true)]
    [Description("Retrieves every order belonging to a customer.")]
    public async Task<IReadOnlyList<OrderDto>> GetCustomerOrdersAsync(
        [Description("GUID identifier of the customer whose orders should be returned.")] Guid customerId,
        CancellationToken cancellationToken)
        => await mediator.Send(new GetCustomerOrdersQuery(customerId), cancellationToken);

    [McpServerTool(Name = "create_order", Title = "Create order", Destructive = false)]
    [Description("Creates a new order for a customer. The order total is calculated from the supplied product prices.")]
    public async Task<string> CreateOrderAsync(
        [Description("GUID identifier of the customer the order belongs to.")] Guid customerId,
        [Description("Products to place on the order.")] IReadOnlyList<ProductDto> products,
        CancellationToken cancellationToken)
    {
        if (products.Count == 0)
        {
            throw new McpException("An order must contain at least one product.");
        }

        CreateOrderDto order = new()
        {
            CustomerId = customerId,
            Products = [.. products]
        };

        await mediator.Send(new CreateOrderCommand(order), cancellationToken);

        return $"Created an order for customer {customerId} with {products.Count} product(s). Call get_customer_orders to retrieve it.";
    }
}
