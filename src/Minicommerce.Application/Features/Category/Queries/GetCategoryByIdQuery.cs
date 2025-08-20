using System;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;

namespace Minicommerce.Application.Features.Category.Queries;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<Result<CategoryDto>>;