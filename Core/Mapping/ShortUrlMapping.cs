using AutoMapper;
using Core.Entities;
using Core.DTOs.ShortUrl;

namespace Core.Mapping;

public class ShortUrlMapping : Profile
{
    public ShortUrlMapping()
    {
        CreateMap<ShortUrl, CreateShortUrlDTO>()
            .ForMember(surl => surl.Id, opt => opt.MapFrom(surl => surl.Id))
            .ForMember(surl => surl.OriginalUrl, opt => opt.MapFrom(surl => surl.OriginalUrl))
            .ForMember(surl => surl.ShortCode, opt => opt.MapFrom(surl => surl.ShortCode));

        CreateMap<ShortUrl, ShortUrlResponseDTO>()
            .ForMember(surl => surl.Id, opt => opt.MapFrom(surl => surl.Id))
            .ForMember(surl => surl.OriginalUrl, opt => opt.MapFrom(surl => surl.OriginalUrl))
            .ForMember(surl => surl.ShortCode, opt => opt.MapFrom(surl => surl.ShortCode));
    }
}