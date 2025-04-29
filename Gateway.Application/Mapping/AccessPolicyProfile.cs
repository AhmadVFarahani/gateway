using AutoMapper;
using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class AccessPolicyProfile : Profile
{
    public AccessPolicyProfile()
    {
        CreateMap<AccessPolicy, AccessPolicyDto>().ReverseMap();
    }
}