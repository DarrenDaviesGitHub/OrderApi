using Ardent.Domain.Models;
using Ardent.OrderApi.DomainTransferObjects;
using MediatR;

namespace Ardent.OrderApi.Commands;

public record CreateOrderCommand(CreateOrderDto order) : IRequest;
