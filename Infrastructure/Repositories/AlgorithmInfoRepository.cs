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

    public async Task UpdateAsync(AlgorithmInfo algorithmInfo)
    {
        context.Infos.Update(algorithmInfo);
    }

    public async Task SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}