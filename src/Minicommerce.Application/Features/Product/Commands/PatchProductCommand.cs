using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Catalog.Products.Models;

namespace Minicommerce.Application.Catalog.Products.Commands;

public record PatchProductCommand(Guid Id, PatchProductDto Product) : IRequest<Result<bool>>;
