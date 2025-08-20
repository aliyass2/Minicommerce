using System;
using MediatR;
using Minicommerce.Application.Common.Models;
using Minicommerce.Application.Features.Category.Dtos;

namespace Minicommerce.Application.Features.Category.Commands;

public record PatchCategoryCommand(Guid Id, PatchCategoryDto Category) : IRequest<Result<bool>>;

