using Ardent.OrderApi.Application.DomainTransferObjects;
using MediatR;

namespace Ardent.OrderApi.Application.Commands;

public record CreateOrderCommand(CreateOrderDto order) : IRequest;