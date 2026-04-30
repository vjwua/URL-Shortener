using AutoMapper;
using Core.DTOs.AlgorithmInfo;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Services;
using Moq;

namespace Tests.Services;

public class AlgorithmInfoServiceTests
{
    private readonly Mock<IAlgorithmInfoRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AlgorithmInfoService _service;

    public AlgorithmInfoServiceTests()
    {
        _repositoryMock = new Mock<IAlgorithmInfoRepository>();
        _mapperMock = new Mock<IMapper>();
        _service = new AlgorithmInfoService(_mapperMock.Object, _repositoryMock.Object);
    }

    // --- GetAsync ---

    [Fact]
    public async Task GetAsync_ExistingInfo_ReturnsMappedDTO()
    {
        // Arrange
        var entity = new AlgorithmInfo { Description = "Base62 algorithm" };
        var dto = new AlgorithmInfoDTO { Description = "Base62 algorithm" };

        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<AlgorithmInfoDTO>(entity)).Returns(dto);

        // Act
        var result = await _service.GetAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Base62 algorithm", result.Description);
    }

    [Fact]
    public async Task GetAsync_NoInfo_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync((AlgorithmInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.GetAsync());
    }

    // --- UpdateAsync ---

    [Fact]
    public async Task UpdateAsync_ExistingInfo_MapsAndSaves()
    {
        // Arrange
        var entity = new AlgorithmInfo { Description = "Old description" };
        var dto = new AlgorithmInfoDTO { Description = "New description" };

        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync(entity);

        // Act
        await _service.UpdateAsync(dto);

        // Assert
        _mapperMock.Verify(m => m.Map(dto, entity), Times.Once);
        _repositoryMock.Verify(r => r.UpdateAsync(entity), Times.Once);
        _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NoInfo_ThrowsKeyNotFoundException()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAsync()).ReturnsAsync((AlgorithmInfo?)null);

        // Act & Assert
        await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _service.UpdateAsync(new AlgorithmInfoDTO()));
    }
}