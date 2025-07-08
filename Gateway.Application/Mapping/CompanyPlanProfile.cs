using AutoMapper;
using Gateway.Application.Company.Dtos;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class CompanyPlanProfile : Profile
{
    public CompanyPlanProfile()
    {
        CreateMap<Domain.Entities.CompanyPlan, CompanyPlanDto>().ReverseMap();
    }
}
