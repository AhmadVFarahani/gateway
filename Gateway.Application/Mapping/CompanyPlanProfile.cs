using AutoMapper;
using Gateway.Application.Company.Dtos;

namespace Gateway.Application.Mapping;

public class CompanyPlanProfile : Profile
{
    public CompanyPlanProfile()
    {
        CreateMap<Domain.Entities.CompanyPlan, CompanyPlanDto>().ReverseMap();
    }
}
