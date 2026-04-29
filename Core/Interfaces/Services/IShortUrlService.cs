using Core.DTOs.ShortUrl;

namespace Core.Interfaces.Services;

public interface IShortUrlService
{
    Task<IEnumerable<ShortUrlResponseDTO>> GetAllAsync();
    Task<ShortUrlResponseDTO?> GetByIdAsync(Guid id);
    Task<string?> GetOriginalUrlByShortCodeAsync(string shortCode);
    Task<ShortUrlResponseDTO> CreateAsync(CreateShortUrlDTO dto, string userId);
    Task DeleteAsync(Guid id, string userId, bool isAdmin);
    Task DeleteAllAsync();
}
