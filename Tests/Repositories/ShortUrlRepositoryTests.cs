using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories;

public class ShortUrlRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ShortUrlRepository _repository;

    public ShortUrlRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new ShortUrlRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAllAsync_ReturnsAllRecords()
    {
        // Arrange
        await _context.ShortUrls.AddRangeAsync(
            new ShortUrl { Id = Guid.NewGuid(), ShortCode = "abc123", OriginalUrl = "https://google.com" },
            new ShortUrl { Id = Guid.NewGuid(), ShortCode = "xyz789", OriginalUrl = "https://github.com" }
        );
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsEntity()
    {
        // Arrange
        var id = Guid.NewGuid();
        await _context.ShortUrls.AddAsync(
            new ShortUrl { Id = id, ShortCode = "abc123", OriginalUrl = "https://google.com" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByShortCodeAsync_ExistingCode_ReturnsEntity()
    {
        // Arrange
        await _context.ShortUrls.AddAsync(
            new ShortUrl { Id = Guid.NewGuid(), ShortCode = "abc123", OriginalUrl = "https://google.com" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByShortCodeAsync("abc123");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.ShortCode);
    }

    [Fact]
    public async Task ExistsAsync_ExistingCode_ReturnsTrue()
    {
        // Arrange
        await _context.ShortUrls.AddAsync(
            new ShortUrl { Id = Guid.NewGuid(), ShortCode = "abc123", OriginalUrl = "https://google.com" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.ExistsAsync("abc123");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_NonExistingCode_ReturnsFalse()
    {
        // Act
        var result = await _repository.ExistsAsync("missing");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task AddAsync_AddsEntityToDatabase()
    {
        // Arrange
        var entity = new ShortUrl
        {
            Id = Guid.NewGuid(),
            ShortCode = "abc123",
            OriginalUrl = "https://google.com"
        };

        // Act
        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(1, await _context.ShortUrls.CountAsync());
    }

    [Fact]
    public async Task DeleteAsync_RemovesEntityFromDatabase()
    {
        // Arrange
        var entity = new ShortUrl
        {
            Id = Guid.NewGuid(),
            ShortCode = "abc123",
            OriginalUrl = "https://google.com"
        };
        await _context.ShortUrls.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAsync(entity);
        await _repository.SaveChangesAsync();

        // Assert
        Assert.Equal(0, await _context.ShortUrls.CountAsync());
    }

    [Fact]
    public async Task DeleteAllAsync_RemovesAllEntities()
    {
        // Arrange
        await _context.ShortUrls.AddRangeAsync(
            new ShortUrl { Id = Guid.NewGuid(), ShortCode = "abc123", OriginalUrl = "https://google.com" },
            new ShortUrl { Id = Guid.NewGuid(), ShortCode = "xyz789", OriginalUrl = "https://github.com" }
        );
        await _context.SaveChangesAsync();

        // Act
        await _repository.DeleteAllAsync();
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(0, await _context.ShortUrls.CountAsync());
    }
}