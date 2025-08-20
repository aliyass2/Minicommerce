// Minicommerce.WebApi/Controllers/Catalog/ProductController.cs
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Catalog.Products.Add;
using Minicommerce.Application.Catalog.Products.Commands;
using Minicommerce.Application.Catalog.Products.Delete;
using Minicommerce.Application.Catalog.Products.GetById;
using Minicommerce.Application.Catalog.Products.List;
using Minicommerce.Application.Catalog.Products.Models;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.WebApi.Controllers.Catalog;

[ApiController]
[Route("api/catalog/products")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProductController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] AddProductCommand command)
    {
        var result = await _mediator.Send(command);

        if (!result.Succeeded)
        {
            if (result.IsConflict)
                return Conflict(new { errors = result.Errors });
            return BadRequest(new { errors = result.Errors });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }
    [HttpPatch("{id}")]
    public async Task<IActionResult> PatchProduct(Guid id, [FromBody] PatchProductDto productDto)
    {
        var command = new PatchProductCommand(id, productDto);
        var result = await _mediator.Send(command);

        if (result.Succeeded)
        {
            return Ok(new
            {
                success = true,
                message = "Patch updated successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors,
            message = "Patch update failed"
        });
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(id));

        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<ProductDto>>> List(
        [FromQuery] string? search,
        [FromQuery] Guid? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? sort = null)
    {
        var result = await _mediator.Send(new ListProductsQuery
        {
            Search = search,
            CategoryId = categoryId,
            Page = page,
            PageSize = pageSize,
            Sort = sort
        });

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id));

        if (result.Succeeded)
        {
            return Ok(new
            {
                success = true,
                message = "Product deleted successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors,
            message = "Product deletion failed"
        });
    }

}
