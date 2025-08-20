using System;
using MediatR;
using Minicommerce.Application.Common.Models;

namespace Minicommerce.Application.Features.Category.Commands;

public record DeleteCategoryCommand(Guid Id) : IRequest<Result>;
