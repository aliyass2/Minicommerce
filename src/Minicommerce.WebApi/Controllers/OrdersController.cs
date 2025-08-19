using MediatR;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Orders.Dtos;

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdersController(IMediator mediator) => _mediator = mediator;

    [HttpPost("from-checkout/{checkoutId:guid}")]
    public Task<OrderDto> CreateFromCheckout(Guid checkoutId) =>
        _mediator.Send(new Minicommerce.Application.Orders.Create.CreateOrderFromCheckoutCommand(checkoutId));
}
