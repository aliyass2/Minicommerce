using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Checkout.Create;
using Minicommerce.Application.Checkout.Dtos;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.CheckOut.Queries;

[ApiController]
[Route("api/checkout")]
public class CheckoutController : ControllerBase
{
    private readonly IMediator _mediator;
    public CheckoutController(IMediator mediator) => _mediator = mediator;

    [HttpGet("my")]
    public async Task<ActionResult<PaginatedList<CheckoutDto>>> ListMine(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        var userId =
            User.FindFirstValue(ClaimTypes.NameIdentifier) ??
            User.FindFirst("sub")?.Value ??
            User.FindFirstValue("userId"); // optional custom claim fallback
        if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

        var result = await _mediator.Send(new GetMyCheckoutQuery(userId)
        {
            Page = page,
            PageSize = pageSize,
            Sort = sort
        });

        if (!result.Succeeded) return BadRequest(new { errors = result.Errors });
        return Ok(result.Data);
    }
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

