using System;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Application.Features.Auth.Commands;
using Minicommerce.Application.Features.Auth.Dtos;
using Minicommerce.Domain.Entities.User;

namespace Minicommerce.Application.Features.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponseDto?>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        ITokenService tokenService,
        ILogger<LoginCommandHandler> logger)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponseDto?> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var loginDto = request.LoginDto;

        try
        {
            // Find user by username
            var user = await _userManager.FindByNameAsync(loginDto.UserName);
            if (user == null)
            {
                _logger.LogWarning("Login attempt failed: User {UserName} not found", loginDto.UserName);
                return null; // User not found
            }

            // Check if user is active
            if (!user.IsActive)
            {
                _logger.LogWarning("Login attempt failed: User {UserName} is deactivated", loginDto.UserName);
                throw new UnauthorizedAccessException("User account is deactivated");
            }

            // Check if user is locked out
            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Login attempt failed: User {UserName} is locked out", loginDto.UserName);
                throw new UnauthorizedAccessException("Account is locked due to multiple failed login attempts");
            }

            // Verify password using UserManager
            var passwordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            
            if (!passwordValid)
            {
                _logger.LogWarning("Login attempt failed: Invalid password for user {UserName}", loginDto.UserName);
                
                // Record failed login attempt
                await _userManager.AccessFailedAsync(user);
                
                return null; // Invalid password
            }

            // Reset access failed count on successful login
            if (await _userManager.GetAccessFailedCountAsync(user) > 0)
            {
                await _userManager.ResetAccessFailedCountAsync(user);
            }

            // Generate JWT token
            var token = await _tokenService.CreateToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            // Update last login time (optional)
            user.UpdatedAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {UserName} logged in successfully", loginDto.UserName);

            return new LoginResponseDto
            {
                Token = token,
                Expires = DateTime.UtcNow.AddDays(7), // Match token expiration
                User = new UserInfoDto
                {
                    Id = user.Id,
                    UserName = user.UserName!,
                    FullName = user.FullName,
                    Position = user.Position,
                    IsActive = user.IsActive,
                    Roles = roles.ToList(),
                    CreatedAt = user.CreatedAt
                }
            };
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw authorization exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during login for user {UserName}", loginDto.UserName);
            throw new InvalidOperationException("An error occurred during login process");
        }
    }
}