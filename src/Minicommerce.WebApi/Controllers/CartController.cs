using MediatR;
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
        // assuming User.Identity.Name or a claim contains userId
        var userId = User.FindFirst("sub")?.Value ?? User.Identity?.Name;

        if (string.IsNullOrWhiteSpace(userId))
            return Unauthorized();

        var result = await _mediator.Send(new GetMyCartQuery(userId));

        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> Add(AddToCartCommand cmd) => Ok(await _mediator.Send(cmd));

    [HttpPut("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> Update(Guid productId, [FromBody] int quantity)
        => Ok(await _mediator.Send(new UpdateCartItemQuantityCommand(productId, quantity)));

    [HttpDelete("items/{productId:guid}")]
    public async Task<ActionResult<CartDto>> Remove(Guid productId)
        => Ok(await _mediator.Send(new RemoveFromCartCommand(productId)));

    [HttpDelete]
    public async Task<ActionResult<CartDto>> Clear()
        => Ok(await _mediator.Send(new ClearCartCommand()));

}
