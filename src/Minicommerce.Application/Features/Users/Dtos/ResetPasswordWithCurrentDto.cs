using System;
using System.ComponentModel.DataAnnotations;

namespace Minicommerce.Application.Features.Users.Dtos;

public class ResetPasswordWithCurrentDto
{
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string CurrentPassword { get; set; } = string.Empty;
    
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string NewPassword { get; set; } = string.Empty;
    
    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Password confirmation does not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}