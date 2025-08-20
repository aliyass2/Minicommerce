using System;
using System.ComponentModel.DataAnnotations;

namespace Minicommerce.Application.Features.Users.Dtos;

public class UpdateUserDto
{
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    [Required]
    public List<string> Roles { get; set; } = new();

    [StringLength(100)]
    public string? Position { get; set; }

    public bool IsActive { get; set; }
}