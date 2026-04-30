using Infrastructure.Identity;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user);
    Task<string> GenerateRefreshTokenAsync(ApplicationUser user);
    Task<AuthResponseDTO> RefreshAsync(string refreshToken);
    Task RevokeAsync(string refreshToken);
}