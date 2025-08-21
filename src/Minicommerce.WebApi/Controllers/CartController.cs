using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Cart.AddItem;
using Minicommerce.Application.Cart.Clear;
using Minicommerce.Application.Cart.RemoveItem;
using Minicommerce.Application.Cart.UpdateQuantity;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Cart.Queries;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Roles = "Admin")]

    public async Task<ActionResult<PaginatedList<CartDto>>> List(
         [FromQuery] string? userid,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 20,
         [FromQuery] string? sort = null)
    {
        var result = await _mediator.Send(new GetCartQuery
        {
            UserId = userid,
            Page = page,
            PageSize = pageSize,
            Sort = sort
        });

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("my")]
    public async Task<ActionResult<CartDto>> GetMyCart()
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirst("sub")?.Value ??
            User.FindFirstValue("userId"); // optional custom claim fallback

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized(new { errors = new[] { "Missing user id claim." } });

        var result = await _mediator.Send(new GetMyCartQuery(userId));

        if (!result.Succeeded || result.Data is null)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> Add([FromBody] AddToCartCommand cmd)
    {
        var result = await _mediator.Send(cmd);

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPut("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> Update(Guid productId, [FromBody] int quantity)
    {
        var result = await _mediator.Send(new UpdateCartItemQuantityCommand(productId, quantity));

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> Remove(Guid productId)
    {
        var result = await _mediator.Send(new RemoveFromCartCommand(productId));

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
    [HttpDelete]
    public async Task<ActionResult<CartDto>> Clear()
    {
        var result = await _mediator.Send(new ClearCartCommand());

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

}
