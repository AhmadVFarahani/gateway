using AutoMapper;
using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.RouteScopes.Dtos;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class AccessPolicyProfile : Profile
{
    public AccessPolicyProfile()
    {
        CreateMap<AccessPolicy, AccessPolicyDto>()
            .ForMember(vm=>vm.ScopeName,m=>m.MapFrom(src=>src.Scope.Name))
            .ForMember(vm=>vm.UserName,m=>m.MapFrom(src => src.User != null ? src.User!.UserName: string.Empty))
            .ForMember(vm => vm.ApiKey, m => m.MapFrom(src => src.ApiKey != null ? src.ApiKey!.Key : string.Empty))
            .ReverseMap();
    }
}
