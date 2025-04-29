using AutoMapper;
using Gateway.Application.ApiKeys;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class ApiKeyProfile : Profile
{
    public ApiKeyProfile()
    {
        CreateMap<ApiKey, ApiKeyDto>().ReverseMap();
    }
}