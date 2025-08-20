using System;

namespace Minicommerce.Application.Features.Auth.Dtos;

public class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expires { get; set; }
    public UserInfoDto User { get; set; } = new();
}
