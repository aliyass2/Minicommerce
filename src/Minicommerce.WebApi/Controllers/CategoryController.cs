using MediatR;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Catalog.Categories.Create;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Commands;
using Minicommerce.Application.Features.Category.Dtos;
using Minicommerce.Application.Features.Category.Queries;

[ApiController]
[Route("api/catalog/categories")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PaginatedList<CategoryDto>>> List(
         [FromQuery] string? search,
         [FromQuery] int page = 1,
         [FromQuery] int pageSize = 20,
         [FromQuery] string? sort = null)
    {
        var result = await _mediator.Send(new GetCategoriesQuery
        {
            Search = search,
            Page = page,
            PageSize = pageSize,
            Sort = sort
        });

        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetCategoryByIdQuery(id));

        if (!result.Succeeded)
            return NotFound(new { errors = result.Errors });

        return Ok(result.Data);
    }
    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] AddCategoryCommand command)
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
    public async Task<IActionResult> PatchCategory(Guid id, [FromBody] PatchCategoryDto categoryDto)
    {
        var command = new PatchCategoryCommand(id, categoryDto);
        var result = await _mediator.Send(command);

        if (result.Succeeded)
        {
            return Ok(new
            {
                success = true,
                message = "Category updated successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors,
            message = "Category update failed"
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteCategory(Guid id)
    {
        var result = await _mediator.Send(new DeleteCategoryCommand(id));

        if (result.Succeeded)
        {
            return Ok(new
            {
                success = true,
                message = "Category deleted successfully"
            });
        }

        return BadRequest(new
        {
            success = false,
            errors = result.Errors,
            message = "Category deletion failed"
        });
    }

}
