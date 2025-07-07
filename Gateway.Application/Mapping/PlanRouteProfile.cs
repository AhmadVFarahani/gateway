using AutoMapper;
using Gateway.Application.Plan.Dtos;

namespace Gateway.Application.Mapping;

public class PlanRouteProfile : Profile
{
    public PlanRouteProfile()
    {
        CreateMap<Domain.Entities.PlanRoute, PlanRouteDto>()
             .ForMember(vm => vm.RoutePath, m => m.MapFrom(src => src.Route.Path))
            .ReverseMap();
    }
}