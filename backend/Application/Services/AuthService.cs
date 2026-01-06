using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services;

public class AuthService(UserManager<IdentityUser> userManager, IConfiguration configuration) : IAuthService
{
    private readonly UserManager<IdentityUser> _userManager = userManager;
    private readonly IConfiguration _configuration = configuration;

    public async Task<AuthResponseDto> LoginAsync(LoginDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            throw new Exception("User not found");
        }

        var result = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!result)
        {
            throw new Exception("Invalid password");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtTokenAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Roles = roles.ToList()
        };
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto request)
    {
        var user = new IdentityUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new Exception($"Registration failed: {errors}");
        }

        // Assign default User role
        await _userManager.AddToRoleAsync(user, "User");

        var roles = await _userManager.GetRolesAsync(user);
        var token = await GenerateJwtTokenAsync(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email!,
            Roles = roles.ToList()
        };
    }

    private async Task<string> GenerateJwtTokenAsync(IdentityUser user)
    {
        var jwtKey = _configuration["Jwt:Key"] ?? "ThisIsASuperSecureKeyForJwtTokenGeneration2024!";
        var key = Encoding.ASCII.GetBytes(jwtKey);

        // Get user roles
        var roles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        // Add role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
