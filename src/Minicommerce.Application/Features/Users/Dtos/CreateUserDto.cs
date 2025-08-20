using System;
using System.ComponentModel.DataAnnotations;

namespace Minicommerce.Application.Features.Users.Dtos;

public class CreateUserDto
{
    [Required]
    [StringLength(256)]
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    [Required]
    public List<string> Roles { get; set; } = new();

    [StringLength(100)]
    public string? Position { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}