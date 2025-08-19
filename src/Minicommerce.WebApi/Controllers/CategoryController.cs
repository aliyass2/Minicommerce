using MediatR;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Catalog.Categories.Create;

[ApiController]
[Route("api/catalog/categories")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;
    public CategoryController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCategoryCommand command)
    {
        var id = await _mediator.Send(command);
        return CreatedAtAction(nameof(Get), new { id }, id);
    }

    [HttpGet("{id:guid}")]
    public ActionResult Get(Guid id) => Ok(new { id }); // placeholder; add real query later
}
