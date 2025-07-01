using AutoMapper;
using Gateway.Application.Plan.Dtos;

namespace Gateway.Application.Mapping;

public class PlanProfile : Profile
{
    public PlanProfile()
    {
        CreateMap<Domain.Entities.Plan, PlanDto>().ReverseMap();
    }
}