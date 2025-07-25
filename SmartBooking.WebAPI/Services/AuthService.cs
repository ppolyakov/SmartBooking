using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartBooking.Infrastructure.Persistence;
using SmartBooking.Shared;
using SmartBooking.WebAPI.Models;
using SmartBooking.WebAPI.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class AuthService(UserManager<ApplicationUser> users, IOptions<JwtSettings> jwtOptions, ILogger<AuthService> logger) : IAuthService
{
    private readonly UserManager<ApplicationUser> _users = users;
    private readonly JwtSettings _jwt = jwtOptions.Value;
    private readonly ILogger<AuthService> _logger = logger;

    public async Task<Result<bool>> RegisterAsync(RegisterDto dto, CancellationToken ct = default)
    {
        if (await _users.FindByEmailAsync(dto.Email) != null)
            return Result<bool>.Failure("Email is already taken.");

        var userResult = await _users.CreateAsync(
            new ApplicationUser { UserName = dto.Email, Email = dto.Email },
            dto.Password);

        if (!userResult.Succeeded)
        {
            var errors = string.Join("; ", userResult.Errors.Select(e => e.Description));
            _logger.LogError("Registration errors for {Email}: {Errors}", dto.Email, errors);
            return Result<bool>.Failure(errors);
        }

        var user = await _users.FindByEmailAsync(dto.Email);

        if (user == null)
        {
            _logger.LogError("User {Email} not found after creation.", dto.Email);
            return Result<bool>.Failure("User creation failed.");
        }

        await _users.AddToRoleAsync(user, "User");

        _logger.LogInformation("User {Email} registered successfully.", dto.Email);
        return Result<bool>.Success(true);
    }

    public async Task<Result<string>> LoginAsync(LoginDto dto, CancellationToken ct = default)
    {
        var user = await _users.FindByEmailAsync(dto.Email);
        if (user == null || !await _users.CheckPasswordAsync(user, dto.Password))
            return Result<string>.Failure("Invalid email or password.");

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var roles = await _users.GetRolesAsync(user);
        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwt.ExpiresMinutes),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Result<string>.Success(jwt);
    }
}