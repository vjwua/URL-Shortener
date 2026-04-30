using Core.Entities;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Repositories;

public class AlgorithmInfoRepositoryTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly AlgorithmInfoRepository _repository;

    public AlgorithmInfoRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new AppDbContext(options);
        _repository = new AlgorithmInfoRepository(_context);
    }

    public void Dispose() => _context.Dispose();

    [Fact]
    public async Task GetAsync_ExistingInfo_ReturnsEntity()
    {
        // Arrange
        await _context.Infos.AddAsync(new AlgorithmInfo { Description = "Base62 algorithm" });
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Base62 algorithm", result.Description);
    }

    [Fact]
    public async Task GetAsync_NoInfo_ReturnsNull()
    {
        // Act
        var result = await _repository.GetAsync();

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ChangesDescription()
    {
        // Arrange
        var entity = new AlgorithmInfo { Description = "Old description" };
        await _context.Infos.AddAsync(entity);
        await _context.SaveChangesAsync();

        // Act
        entity.Description = "New description";
        await _repository.UpdateAsync(entity);
        await _repository.SaveChangesAsync();

        // Assert
        var updated = await _context.Infos.FirstOrDefaultAsync();
        Assert.Equal("New description", updated!.Description);
    }
}