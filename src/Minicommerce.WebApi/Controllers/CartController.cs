using MediatR;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Cart.AddItem;
using Minicommerce.Application.Cart.Clear;
using Minicommerce.Application.Cart.RemoveItem;
using Minicommerce.Application.Cart.UpdateQuantity;

[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    public CartController(IMediator mediator) => _mediator = mediator;


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
