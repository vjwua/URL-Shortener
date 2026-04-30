using AutoMapper;
using Core.DTOs.ShortUrl;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Core.Services;

public class ShortUrlService(
    IMapper mapper,
    IShortUrlRepository shortUrlRepository,
    IMemoryCache cache) : IShortUrlService
{
    private static string CacheKey(string code) => $"shorturl:{code}";

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
        if (cache.TryGetValue(CacheKey(shortCode), out string? cachedUrl))
            return cachedUrl;

        var link = await shortUrlRepository.GetByShortCodeAsync(shortCode);

        if (link is null)
            throw new KeyNotFoundException($"Short URL with code '{shortCode}' was not found.");

        cache.Set(CacheKey(shortCode), link.OriginalUrl, TimeSpan.FromMinutes(30));

        return link.OriginalUrl;
    }

    public async Task<ShortUrlResponseDTO> CreateAsync(CreateShortUrlDTO dto, string userId)
    {
        var shortCode = GenerateUniqueShortCodeAsync();

        var shortUrl = new ShortUrl
        {
            OriginalUrl = dto.OriginalUrl,
            ShortCode = shortCode.Result,
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

        cache.Remove(CacheKey(link.ShortCode));

        await shortUrlRepository.DeleteAsync(link);
        await shortUrlRepository.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        var links = await shortUrlRepository.GetAllAsync();

        foreach (var link in links)
        {
            cache.Remove(CacheKey(link.ShortCode));
        }

        await shortUrlRepository.DeleteAllAsync();
        await shortUrlRepository.SaveChangesAsync();
    }

    private async Task<string> GenerateUniqueShortCodeAsync()
    {
        const int maxAttempts = 5;

        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            var code = GenerateShortCode();
            var exists = await shortUrlRepository.ExistsAsync(code);

            if (!exists)
                return code;
        }

        throw new InvalidOperationException(
            "Failed to generate a unique short code. Please try again.");
    }

    private static string GenerateShortCode()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }
}