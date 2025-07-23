using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SmartBooking.Infrastructure.Identity;
using SmartBooking.Shared;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartBooking.WebAPI.Services;

public class AuthService(UserManager<ApplicationUser> userManager, IConfiguration config, ILogger<AuthService> logger) : IAuthService
{
    public async Task<Result<bool>> RegisterAsync(RegisterDto registerDto)
    {
        if (await userManager.FindByEmailAsync(registerDto.Email) != null)
        {
            logger.LogWarning("User with email {Email} already exists.", registerDto.Email);
            return Result<bool>.Failure("User with this email already exists.");
        }

        var user = new ApplicationUser { UserName = registerDto.Email, Email = registerDto.Email };
        var result = await userManager.CreateAsync(user, registerDto.Password);

        if (!result.Succeeded)
        {
            logger.LogError("User registration failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return Result<bool>.Failure("User registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        await userManager.AddToRoleAsync(user, "User");
        return Result<bool>.Success(true);
    }
    public async Task<Result<string>> LoginAsync(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, loginDto.Password))
        {
            logger.LogWarning("Invalid login attempt for email {Email}.", loginDto.Email);
            return Result<string>.Failure("Invalid email or password.");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };
        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var jwtSection = config.GetSection("Jwt");
        var keyBytes = Encoding.UTF8.GetBytes(jwtSection["Key"]!);

        var creds = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSection["Issuer"],
            audience: jwtSection["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Result<string>.Success(tokenString);
    }
}