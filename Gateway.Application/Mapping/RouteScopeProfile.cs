using AutoMapper;
using Gateway.Application.RouteScopes.Dtos;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class RouteScopeProfile : Profile
{
    public RouteScopeProfile()
    {
        CreateMap<RouteScope, RouteScopeDto>().ForMember(vm=>vm.ServiceName,m=>m.MapFrom(src=>src.Route.Service!.Name)).ReverseMap();
    }
}