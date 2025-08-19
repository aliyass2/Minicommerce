using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Catalog.Products.Models;

namespace Minicommerce.Application.Catalog.Products.GetById;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<Result<ProductDto>>;