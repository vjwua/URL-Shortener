using Core.DTOs.AlgorithmInfo;

namespace Core.Interfaces.Services;

public interface IAlgorithmInfoService
{
    Task<AlgorithmInfoDTO?> GetAsync();
    Task UpdateAsync(AlgorithmInfoDTO dto);
}
