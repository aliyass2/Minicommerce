using MediatR;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Checkout.Create;
using Minicommerce.Application.Checkout.Dtos;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly IMediator _mediator;
    public CheckoutController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<CheckoutDto>> Create()
    {
        var result = await _mediator.Send(new CreateCheckoutFromCartCommand());

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }
    [HttpPost("{checkoutId:guid}/pay")]
    public async Task<ActionResult<CheckoutDto>> Pay(Guid checkoutId)
    {
        const string defaultPaymentMethod = "CashOnDelivery";
        var result = await _mediator.Send(
            new Minicommerce.Application.Checkout.Pay.PayCheckoutCommand(
                checkoutId, defaultPaymentMethod));

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

[HttpPost("{checkoutId:guid}/complete")]
public async Task<ActionResult<CheckoutDto>> Complete(Guid checkoutId)
{
    var result = await _mediator.Send(
        new Minicommerce.Application.Checkout.Complete.CompleteCheckoutCommand(checkoutId));

    if (!result.Succeeded)
        return BadRequest(new { errors = result.Errors });

    return Ok(result.Data);
}}

