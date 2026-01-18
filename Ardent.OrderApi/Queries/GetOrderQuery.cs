using Ardent.Domain.Models;
using MediatR;

namespace Ardent.OrderApi.Queries;

public record GetOrderQuery(Guid OrderId, Guid CustomerId) : IRequest<Order>;