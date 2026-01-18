using Ardent.Domain.Models;
using Ardent.OrderApi.Commands;
using Ardent.OrderApi.DomainTransferObjects;
using Ardent.OrderApi.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Ardent.OrderApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class OrderController(IMediator mediator) : ControllerBase
{
    [HttpGet("customer/{customerId}/order/{orderId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Order>> Get(Guid orderId, Guid customerId)
    {
        var result = await mediator.Send(new GetOrderQuery(orderId, customerId));
        return result is not null ? Ok(result) : NotFound();
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Post([FromBody] CreateOrderDto order)
    {
        await mediator.Send(new CreateOrderCommand(order));
        return StatusCode(StatusCodes.Status201Created);
    }
}