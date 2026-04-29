using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ShortUrlRepository(AppDbContext context) : IShortUrlRepository
{
    public async Task<IEnumerable<ShortUrl>> GetAllAsync()
    {
        return await context.ShortUrls.ToListAsync();
    }

    public async Task<ShortUrl?> GetByIdAsync(Guid id)
    {
        return await context.ShortUrls.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
    {
        return await context.ShortUrls.FirstOrDefaultAsync(x => x.ShortCode == shortCode);
    }

    public async Task AddAsync(ShortUrl shortUrl)
    {
        await context.ShortUrls.AddAsync(shortUrl);
    }

    public async Task DeleteAsync(ShortUrl shortUrl)
    {
        context.ShortUrls.Remove(shortUrl);
    }

    public async Task DeleteAllAsync()
    {
        await context.ShortUrls.ExecuteDeleteAsync();
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}