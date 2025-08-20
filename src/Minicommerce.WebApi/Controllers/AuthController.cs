using System;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Minicommerce.Application.Features.Auth.Commands;
using Minicommerce.Application.Features.Auth.Dtos;
using Minicommerce.Domain.Entities.User;
using Minicommerce.Domain.Repositories;

namespace Minicommerce.WebApi.Controllers;


[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<AuthController> _logger;
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(IMediator mediator, ILogger<AuthController> logger, IUnitOfWork unitOfWork,
        UserManager<ApplicationUser> userManager)
    {
        _mediator = mediator;
        _logger = logger;
        _unitOfWork = unitOfWork;
        _userManager = userManager;

    }
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponseDto), 200)]
    [ProducesResponseType(typeof(object), 401)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 500)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            _logger.LogInformation("Login attempt for user: {UserName}", loginDto.UserName);

            var command = new LoginCommand { LoginDto = loginDto };
            var result = await _mediator.Send(command);

            if (result == null)
            {
                _logger.LogWarning("Login failed for user: {UserName} - Invalid credentials", loginDto.UserName);
                return Unauthorized(new { 
                    message = "Invalid username or password",
                    timestamp = DateTime.UtcNow 
                });
            }

            _logger.LogInformation("Login successful for user: {UserName}", loginDto.UserName);
            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning("Login unauthorized for user: {UserName} - {Message}", loginDto.UserName, ex.Message);
            return Unauthorized(new { 
                message = ex.Message,
                timestamp = DateTime.UtcNow 
            });
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("Login validation failed for user: {UserName}", loginDto.UserName);
            return BadRequest(new { 
                message = "Validation failed", 
                errors = ex.Errors,
                timestamp = DateTime.UtcNow 
            });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError(ex, "Login operation failed for user: {UserName}", loginDto.UserName);
            return StatusCode(500, new { 
                message = ex.Message,
                timestamp = DateTime.UtcNow 
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during login for user: {UserName}", loginDto.UserName);
            return StatusCode(500, new { 
                message = "An unexpected error occurred during login",
                timestamp = DateTime.UtcNow 
            });
        }
    }

[HttpGet("me")]
[Authorize]
[ProducesResponseType(typeof(UserInfoDto), 200)]
[ProducesResponseType(typeof(object), 401)]
public async Task<ActionResult<UserInfoDto>> GetCurrentUser()
{
    try
    {
        // Try multiple claim types for user ID
        var userId = User.FindFirst("sub")?.Value ??
                    User.FindFirst("nameid")?.Value ??
                    User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Unable to determine current user from token claims");
            return Unauthorized(new
            {
                message = "Unable to determine current user",
                timestamp = DateTime.UtcNow
            });
        }

        // Get user from database
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found in database for ID: {UserId}", userId);
            return NotFound(new
            {
                message = "User not found",
                timestamp = DateTime.UtcNow
            });
        }

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        var userInfo = new UserInfoDto
        {
            Id = user.Id,
            UserName = user.UserName ?? string.Empty,
            FullName = user.FullName ?? string.Empty,
            Position = user.Position,
            IsActive = user.IsActive,
            Roles = roles.ToList(),
            CreatedAt = user.CreatedAt
        };

        _logger.LogInformation("Successfully retrieved user information for user ID: {UserId}", userId);
        return Ok(userInfo);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error occurred while retrieving current user information");
        return StatusCode(500, new
        {
            message = "An error occurred while retrieving user information",
            timestamp = DateTime.UtcNow
        });
    }
}
}