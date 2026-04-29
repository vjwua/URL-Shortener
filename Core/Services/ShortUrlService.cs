using AutoMapper;
using Core.DTOs.ShortUrl;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Core.Services;

public class ShortUrlService(
    IMapper mapper,
    IShortUrlRepository shortUrlRepository) : IShortUrlService
{
    public async Task<IEnumerable<ShortUrlResponseDTO>> GetAllAsync()
    {
        var links = await shortUrlRepository.GetAllAsync();
        return mapper.Map<IEnumerable<ShortUrlResponseDTO>>(links);
    }

    public async Task<ShortUrlResponseDTO?> GetByIdAsync(Guid id)
    {
        var link = await shortUrlRepository.GetByIdAsync(id);

        if (link is null)
            throw new KeyNotFoundException($"Short URL with ID '{id}' was not found.");

        return mapper.Map<ShortUrlResponseDTO>(link);
    }

    public async Task<string?> GetOriginalUrlByShortCodeAsync(string shortCode)
    {
        var link = await shortUrlRepository.GetByShortCodeAsync(shortCode);

        if (link is null)
            throw new KeyNotFoundException($"Short URL with code '{shortCode}' was not found.");

        return link.OriginalUrl;
    }

    public async Task<ShortUrlResponseDTO> CreateAsync(CreateShortUrlDTO dto, string userId)
    {
        var shortCode = GenerateShortCode();

        var shortUrl = new ShortUrl
        {
            ShortCode = shortCode,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await shortUrlRepository.AddAsync(shortUrl);
        await shortUrlRepository.SaveChangesAsync();

        return mapper.Map<ShortUrlResponseDTO>(shortUrl);
    }

    public async Task DeleteAsync(Guid id, string userId, bool isAdmin)
    {
        var link = await shortUrlRepository.GetByIdAsync(id);

        if (link is null)
            throw new KeyNotFoundException($"Short URL with ID '{id}' was not found.");

        if (!isAdmin && link.CreatedByUserId != userId)
            throw new UnauthorizedAccessException("You do not have permission to delete this URL.");

        await shortUrlRepository.DeleteAsync(link);
        await shortUrlRepository.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await shortUrlRepository.DeleteAllAsync();
        await shortUrlRepository.SaveChangesAsync();
    }

    private static string GenerateShortCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, 8)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }
}