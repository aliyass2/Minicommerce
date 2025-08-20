using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;

namespace Minicommerce.Application.Catalog.Categories.Create;
public sealed record AddCategoryCommand(string Name) : IRequest<Result<CategoryDto>>;
