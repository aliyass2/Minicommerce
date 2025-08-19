// Minicommerce.WebApi/Controllers/Catalog/ProductController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Catalog.Products.Add;
using Minicommerce.Application.Catalog.Products.Models;

namespace Minicommerce.WebApi.Controllers.Catalog;

[ApiController]
[Route("api/catalog/products")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductController(IMediator mediator) => _mediator = mediator;

    // [Authorize(Roles = "Admin")] // uncomment if you want only admins to add products
    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] AddProductCommand command)
    {
        var dto = await _mediator.Send(command);
        // classic REST: return 201 with a location
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    // quick read endpoint so you can verify insert worked
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById([FromServices] IMediator mediator, Guid id)
    {
        // plug a real query later; for now a placeholder keeps CreatedAtAction happy
        return Ok(null);
    }
}
