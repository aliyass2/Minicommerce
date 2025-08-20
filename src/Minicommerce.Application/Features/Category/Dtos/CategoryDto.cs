using System;

namespace Minicommerce.Application.Features.Category.Dtos;

public sealed class CategoryDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = default!;
}
