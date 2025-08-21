using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Order.Queries;
using Minicommerce.Application.Orders.Create;
using Minicommerce.Application.Orders.Dtos;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;
    [HttpGet]
    [Authorize(Roles = "Admin")]
        public async Task<ActionResult<PaginatedList<OrderDto>>> List(
        [FromQuery] string? userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        var result = await _mediator.Send(new ListOrdersQuery
        {
            UserId = userId,
            Page = page,
            PageSize = pageSize,
            Sort = sort
        });

        if (!result.Succeeded) return BadRequest(new { errors = result.Errors });
        return Ok(result.Data);
    }

    // GET /api/orders/my
    [HttpGet("my")]
    public async Task<ActionResult<PaginatedList<OrderDto>>> ListMine(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirst("sub")?.Value ??
            User.FindFirstValue("userId"); // optional custom claim fallback
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _mediator.Send(new ListMyOrdersQuery(userId)
        {
            Page = page,
            PageSize = pageSize,
            Sort = sort
        });

        if (!result.Succeeded) return BadRequest(new { errors = result.Errors });
        return Ok(result.Data);
    }

[HttpPost("from-checkout/{checkoutId:guid}")]
public async Task<ActionResult<OrderDto>> CreateFromCheckout(Guid checkoutId)
{
    var result = await _mediator.Send(new CreateOrderFromCheckoutCommand(checkoutId));

    if (!result.Succeeded)
        return BadRequest(new { errors = result.Errors });

    return Ok(result.Data);
}}
