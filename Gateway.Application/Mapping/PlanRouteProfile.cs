using AutoMapper;
using Gateway.Application.Plan.Dtos;

namespace Gateway.Application.Mapping;

public class PlanRouteProfile : Profile
{
    public PlanRouteProfile()
    {
        CreateMap<Domain.Entities.PlanRoute, PlanRouteDto>().ReverseMap();
    }
}