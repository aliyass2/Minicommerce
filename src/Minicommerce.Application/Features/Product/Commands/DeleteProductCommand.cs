using System;
using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Catalog.Products.Delete;

public record DeleteProductCommand(Guid Id) : IRequest<Result>;
