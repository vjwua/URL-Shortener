using Infrastructure.Identity;

namespace Infrastructure.Entities;

public class RefreshToken
{
    public Guid Id { get; set; }
    public string? Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
}