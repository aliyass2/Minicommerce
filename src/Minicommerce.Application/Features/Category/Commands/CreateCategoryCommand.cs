using MediatR;

namespace Minicommerce.Application.Catalog.Categories.Create;
public sealed record CreateCategoryCommand(string Name) : IRequest<Guid>;
