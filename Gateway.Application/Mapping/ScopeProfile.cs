using AutoMapper;
using Gateway.Application.Scopes;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class ScopeProfile : Profile
{
    public ScopeProfile()
    {
        CreateMap<Scope, ScopeDto>().ReverseMap();
    }
}