using AutoMapper;
using Core.Entities;
using Core.DTOs.AlgorithmInfo;

namespace Core.Mapping;

public class AlgorithmInfoMapping : Profile
{
    public AlgorithmInfoMapping()
    {
        CreateMap<AlgorithmInfo, AlgorithmInfoDTO>()
            .ForMember(alg => alg.Id, opt => opt.MapFrom(alg => alg.Id))
            .ForMember(alg => alg.Description, opt => opt.MapFrom(alg => alg.Description));
    }
}