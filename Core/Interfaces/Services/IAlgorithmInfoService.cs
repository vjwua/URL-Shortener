using Core.DTOs.AlgorithmInfo;

namespace Core.Interfaces.Services;

public interface IAlgorithmInfoService
{
    Task<AlgorithmInfoDTO?> GetAsync();
    Task CreateAsync(AlgorithmInfoDTO dto);
    Task UpdateAsync(AlgorithmInfoDTO dto);
}
