using AutoMapper;
using Gateway.Application.Routes.Dtos;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class RouteProfile : Profile
{
    public RouteProfile()
    {
        CreateMap<Route, RouteDto>().ReverseMap();
    }
}
