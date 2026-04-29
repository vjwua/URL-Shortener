using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IShortUrlRepository
{
    Task<IEnumerable<ShortUrl>> GetAllAsync();
    Task<ShortUrl?> GetByIdAsync(Guid id);
    Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
    Task AddAsync(ShortUrl shortUrl);
    Task DeleteAsync(ShortUrl shortUrl);
    Task DeleteAllAsync();
    Task SaveChangesAsync();
}
