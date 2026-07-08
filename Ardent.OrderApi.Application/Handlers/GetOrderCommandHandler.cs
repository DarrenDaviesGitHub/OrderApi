using Ardent.OrderApi.Application.DomainTransferObjects;
using Ardent.OrderApi.Application.Queries;
using Ardent.OrderApi.Application.Interfaces;
using AutoMapper;
using MediatR;

namespace Ardent.OrderApi.Application.Handlers;

public class GetOrderQueryHandler(
    IMapper mapper,
    ICosmosOrderRepository orderRepository)
    : IRequestHandler<GetOrderQuery, OrderDto?>
{
    public async Task<OrderDto?> Handle(
        GetOrderQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        Console.WriteLine($"Handling GetOrdersQuery for OrderId: {request.OrderId} and CustomerId: {request.CustomerId}");
        
        var order = await orderRepository.GetOrderAsync(request.OrderId, request.CustomerId, cancellationToken);

        return order is not null
            ? mapper.Map<OrderDto>(order)
            : null;
    }
}