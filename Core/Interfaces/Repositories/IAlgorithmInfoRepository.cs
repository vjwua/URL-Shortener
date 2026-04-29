using Core.Entities;

namespace Core.Interfaces.Repositories;

public interface IAlgorithmInfoRepository
{
    Task<AlgorithmInfo?> GetAsync();
    Task UpdateAsync(AlgorithmInfo algorithmInfo);
    Task SaveChangesAsync();
}
