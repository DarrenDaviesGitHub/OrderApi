using Ardent.OrderApi.Application.DomainTransferObjects;
using MediatR;

namespace Ardent.OrderApi.Application.Queries;

public record GetCustomerOrdersQuery(Guid CustomerId) : IRequest<IReadOnlyList<OrderDto>>;
