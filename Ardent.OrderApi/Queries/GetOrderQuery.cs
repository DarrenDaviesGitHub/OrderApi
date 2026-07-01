using Ardent.Domain.Models;
using Ardent.OrderApi.DomainTransferObjects;
using MediatR;

namespace Ardent.OrderApi.Queries;

public record GetOrderQuery(Guid OrderId, Guid CustomerId) : IRequest<OrderDto>;