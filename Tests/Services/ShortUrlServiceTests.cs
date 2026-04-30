using AutoMapper;
using Core.DTOs.ShortUrl;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace Tests.Services;

public class ShortUrlServiceTests
{
    private readonly Mock<IShortUrlRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMemoryCache> _cacheMock;
    private readonly ShortUrlService _service;

    public ShortUrlServiceTests()
    {
        _repositoryMock = new Mock<IShortUrlRepository>();
        _mapperMock = new Mock<IMapper>();
        _cacheMock = new Mock<IMemoryCache>();

        _service = new ShortUrlService(
            _mapperMock.Object,
            _repositoryMock.Object,
            _cacheMock.Object);
    }

    // --- GetAllAsync ---

    [Fact]
    public async Task GetAllAsync_ReturnsAllMappedLinks()
    {
        // Arrange
        var entities = new List<ShortUrl>
        {
            new() { Id = Guid.NewGuid(), ShortCode = "abc123", OriginalUrl = "https://google.com" },
            new() { Id = Guid.NewGuid(), ShortCode = "xyz789", OriginalUrl = "https://github.com" }
        };
        var dtos = entities.Select(e => new ShortUrlResponseDTO { ShortCode = e.ShortCode }).ToList();

        _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(entities);
        _mapperMock.Setup(m => m.Map<IEnumerable<ShortUrlResponseDTO>>(entities)).Returns(dtos);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count());
    }

    // --- GetByIdAsync ---

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsMappedDTO()
    {
        // Arrange
        var id = Guid.NewGuid();
        var entity = new ShortUrl { Id = id, ShortCode = "abc123", OriginalUrl = "https://google.com" };
        var dto = new ShortUrlResponseDTO { ShortCode = "abc123" };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<ShortUrlResponseDTO>(entity)).Returns(dto);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("abc123", result.ShortCode);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ShortUrl?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetByIdAsync(id));
    }

    // --- GetOriginalUrlByShortCodeAsync ---

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_CacheHit_ReturnsCachedUrl()
    {
        // Arrange
        const string shortCode = "abc123";
        const string cachedUrl = "https://google.com";
        object? cached = cachedUrl;

        _cacheMock.Setup(c => c.TryGetValue($"shorturl:{shortCode}", out cached)).Returns(true);

        // Act
        var result = await _service.GetOriginalUrlByShortCodeAsync(shortCode);

        // Assert
        Assert.Equal(cachedUrl, result);
        _repositoryMock.Verify(r => r.GetByShortCodeAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_CacheMiss_ReturnsFromRepository()
    {
        // Arrange
        const string shortCode = "abc123";
        var entity = new ShortUrl { ShortCode = shortCode, OriginalUrl = "https://google.com" };
        object? cached = null;

        _cacheMock.Setup(c => c.TryGetValue($"shorturl:{shortCode}", out cached)).Returns(false);
        _cacheMock.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());
        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync(entity);

        // Act
        var result = await _service.GetOriginalUrlByShortCodeAsync(shortCode);

        // Assert
        Assert.Equal("https://google.com", result);
    }

    [Fact]
    public async Task GetOriginalUrlByShortCodeAsync_NonExistingCode_ThrowsKeyNotFoundException()
    {
        // Arrange
        const string shortCode = "missing";
        object? cached = null;

        _cacheMock.Setup(c => c.TryGetValue($"shorturl:{shortCode}", out cached)).Returns(false);
        _repositoryMock.Setup(r => r.GetByShortCodeAsync(shortCode)).ReturnsAsync((ShortUrl?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.GetOriginalUrlByShortCodeAsync(shortCode));
    }

    // --- CreateAsync ---

    [Fact]
    public async Task CreateAsync_ValidInput_ReturnsCreatedDTO()
    {
        // Arrange
        var dto = new CreateShortUrlDTO { OriginalUrl = "https://google.com" };
        var userId = Guid.NewGuid().ToString();
        var entity = new ShortUrl { OriginalUrl = dto.OriginalUrl };
        var responseDto = new ShortUrlResponseDTO { OriginalUrl = dto.OriginalUrl };

        _repositoryMock.Setup(r => r.ExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _mapperMock.Setup(m => m.Map<ShortUrl>(dto)).Returns(entity);

        // It.IsAny<ShortUrl>() замість конкретного entity
        _mapperMock.Setup(m => m.Map<ShortUrlResponseDTO>(It.IsAny<ShortUrl>())).Returns(responseDto);

        // Act
        var result = await _service.CreateAsync(dto, userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("https://google.com", result.OriginalUrl);
        _repositoryMock.Verify(r => r.AddAsync(It.IsAny<ShortUrl>()), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    // --- DeleteAsync ---

    [Fact]
    public async Task DeleteAsync_OwnerDeletes_Succeeds()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();
        var entity = new ShortUrl { Id = id, ShortCode = "abc123", CreatedByUserId = userId };
        object? cached = null;

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _cacheMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out cached)).Returns(false);

        // Act
        await _service.DeleteAsync(id, userId, isAdmin: false);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonOwnerDeletes_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var ownerId = Guid.NewGuid().ToString();
        var otherUserId = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();
        var entity = new ShortUrl { Id = id, ShortCode = "abc123", CreatedByUserId = ownerId };

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(
            () => _service.DeleteAsync(id, otherUserId, isAdmin: false));
    }

    [Fact]
    public async Task DeleteAsync_AdminDeletes_Succeeds()
    {
        // Arrange
        var ownerId = Guid.NewGuid().ToString();
        var adminId = Guid.NewGuid().ToString();
        var id = Guid.NewGuid();
        var entity = new ShortUrl { Id = id, ShortCode = "abc123", CreatedByUserId = ownerId };
        object? cached = null;

        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(entity);
        _cacheMock.Setup(c => c.TryGetValue(It.IsAny<string>(), out cached)).Returns(false);

        // Act
        await _service.DeleteAsync(id, adminId, isAdmin: true);

        // Assert
        _repositoryMock.Verify(r => r.DeleteAsync(entity), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistingId_ThrowsKeyNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(id)).ReturnsAsync((ShortUrl?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.DeleteAsync(id, "userId", isAdmin: false));
    }

    // --- DeleteAllAsync ---

    [Fact]
    public async Task DeleteAllAsync_CallsRepositoryAndSavesChanges()
    {
        // Act
        await _service.DeleteAllAsync();

        // Assert
        _repositoryMock.Verify(r => r.DeleteAllAsync(), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}