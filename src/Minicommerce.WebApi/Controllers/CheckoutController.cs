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
    public Task<CheckoutDto> Create() =>
        _mediator.Send(new CreateCheckoutFromCartCommand());

    [HttpPost("{checkoutId:guid}/pay")]
    public Task<CheckoutDto> Pay(Guid checkoutId, [FromBody] PayCheckoutBody body) =>
        _mediator.Send(new Minicommerce.Application.Checkout.Pay.PayCheckoutCommand(checkoutId, body.PaymentMethod, body.TransactionId));

    [HttpPost("{checkoutId:guid}/complete")]
    public Task<CheckoutDto> Complete(Guid checkoutId) =>
        _mediator.Send(new Minicommerce.Application.Checkout.Complete.CompleteCheckoutCommand(checkoutId));
}

public sealed record PayCheckoutBody(string PaymentMethod, string? TransactionId);
