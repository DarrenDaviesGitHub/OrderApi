using Ardent.OrderApi.Commands;
using Ardent.OrderApi.DomainTransferObjects;
using Ardent.OrderApi.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.OrderApi.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpGet("{orderId}/customer/{customerId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderDto>> Get(
        Guid orderId, 
        Guid customerId,
        CancellationToken cancellationToken)
    {
        OrderDto result = await mediator.Send(new GetOrderQuery(orderId, customerId), cancellationToken);
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post(
        [FromBody] CreateOrderDto order,
        CancellationToken cancellationToken)
    {
        await mediator.Send(new CreateOrderCommand(order), cancellationToken);
        return StatusCode(StatusCodes.Status201Created);
    }
}