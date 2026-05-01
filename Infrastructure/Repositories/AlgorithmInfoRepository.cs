using Core.Entities;
using Core.Interfaces.Repositories;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlgorithmInfoRepository(AppDbContext context) : IAlgorithmInfoRepository
{
    public async Task<AlgorithmInfo?> GetAsync()
    {
        return await context.Infos.FirstOrDefaultAsync();
    }

    public Task AddAsync(AlgorithmInfo algorithmInfo)
    {
        context.Infos.Add(algorithmInfo);
        return Task.CompletedTask;
    }
    public async Task UpdateAsync(AlgorithmInfo algorithmInfo)
    {
        context.Infos.Update(algorithmInfo);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}