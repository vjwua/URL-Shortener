using Infrastructure.Data;
using Infrastructure.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Core.Services;

public class TokenService(
    UserManager<ApplicationUser> userManager,
    AppDbContext context,
    IConfiguration configuration) : ITokenService
{
    public async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.UserName!)
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // короткий термін
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync(ApplicationUser user)
    {
        // інвалідуємо попередні токени користувача
        var existing = await context.RefreshTokens
            .Where(t => t.UserId == user.Id && !t.IsRevoked)
            .ToListAsync();

        existing.ForEach(t => t.IsRevoked = true);

        var token = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false,
            UserId = user.Id
        };

        await context.RefreshTokens.AddAsync(token);
        await context.SaveChangesAsync();

        return token.Token;
    }

    public async Task<AuthResponseDTO> RefreshAsync(string refreshToken)
    {
        var token = await context.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token is null || token.IsRevoked)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        if (token.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token has expired.");

        // ротація — старий інвалідується, видається новий
        token.IsRevoked = true;
        await context.SaveChangesAsync();

        var newAccessToken = await GenerateAccessTokenAsync(token.User);
        var newRefreshToken = await GenerateRefreshTokenAsync(token.User);

        return new AuthResponseDTO
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task RevokeAsync(string refreshToken)
    {
        var token = await context.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken);

        if (token is null || token.IsRevoked)
            throw new UnauthorizedAccessException("Invalid refresh token.");

        token.IsRevoked = true;
        await context.SaveChangesAsync();
    }
}