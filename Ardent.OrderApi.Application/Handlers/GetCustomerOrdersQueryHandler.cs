using Ardent.OrderApi.Application.DomainTransferObjects;
using Ardent.OrderApi.Application.Interfaces;
using Ardent.OrderApi.Application.Queries;
using AutoMapper;
using MediatR;

namespace Ardent.OrderApi.Application.Handlers;

public class GetCustomerOrdersQueryHandler(
    IMapper mapper,
    ICosmosOrderRepository orderRepository)
    : IRequestHandler<GetCustomerOrdersQuery, IReadOnlyList<OrderDto>>
{
    public async Task<IReadOnlyList<OrderDto>> Handle(
        GetCustomerOrdersQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var orders = await orderRepository.GetCustomerOrdersAsync(request.CustomerId, cancellationToken);

        return [.. orders.Select(mapper.Map<OrderDto>)];
    }
}