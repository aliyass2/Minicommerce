using System;
using System.ComponentModel.DataAnnotations;

namespace Minicommerce.Application.Features.Users.Dtos;

public class ResetPasswordDto
{
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;
}