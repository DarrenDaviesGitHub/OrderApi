using Ardent.OrderApi.Application.DomainTransferObjects;
using MediatR;

namespace Ardent.OrderApi.Application.Queries;

public record GetOrderQuery(Guid OrderId, Guid CustomerId) : IRequest<OrderDto>;
