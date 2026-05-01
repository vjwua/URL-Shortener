using AutoMapper;
using Core.DTOs.AlgorithmInfo;
using Core.DTOs.ShortUrl;
using Core.Entities;
using Core.Interfaces.Repositories;
using Core.Interfaces.Services;

namespace Core.Services;

public class AlgorithmInfoService(
    IMapper mapper,
    IAlgorithmInfoRepository algorithmInfoRepository) : IAlgorithmInfoService
{
    public async Task<AlgorithmInfoDTO?> GetAsync()
    {
        var info = await algorithmInfoRepository.GetAsync();

        if (info is null)
            throw new KeyNotFoundException("Algorithm description was not found.");

        return mapper.Map<AlgorithmInfoDTO>(info);
    }

    public async Task CreateAsync(AlgorithmInfoDTO dto)
    {
        var info = mapper.Map<AlgorithmInfo>(dto);
        await algorithmInfoRepository.AddAsync(info);
        await algorithmInfoRepository.SaveChangesAsync();
    }

    public async Task UpdateAsync(AlgorithmInfoDTO dto)
    {
        var info = await algorithmInfoRepository.GetAsync();

        if (info is null)
            throw new KeyNotFoundException("Algorithm description was not found.");

        mapper.Map(dto, info);

        await algorithmInfoRepository.UpdateAsync(info);
        await algorithmInfoRepository.SaveChangesAsync();
    }
}