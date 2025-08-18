using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Minicommerce.Application.Common.Interfaces;
using Minicommerce.Domain.Entities.User;

namespace Minicommerce.Infrastructure.Services
{
    public class TokenService(IConfiguration configuration, UserManager<ApplicationUser> userManager) : ITokenService
    {
        public async Task<string> CreateToken(ApplicationUser user)
        {
            var tokenKey = configuration["TokenKey"] ?? throw new Exception("Cannot Access Token Key from app settings");
            if (tokenKey.Length < 64) throw new Exception("Your tokenkey needs to be longer");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));

            if (user.UserName == null) throw new Exception("No Username For User");

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.GivenName, user.FullName),
                new("IsActive", user.IsActive.ToString()),

            };

            if (!string.IsNullOrEmpty(user.Position))
            {
                claims.Add(new Claim("Position", user.Position));
            }


            // Get Identity roles and add them to claims
            var roles = await userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}