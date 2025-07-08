using AutoMapper;
using Gateway.Application.Company.Dtos;

namespace Gateway.Application.Mapping;

public class CompanyRoutePriceinProfile : Profile
{
    public CompanyRoutePriceinProfile()
    {
        CreateMap<Domain.Entities.CompanyRoutePricing, CompanyRoutePricingDto>().ReverseMap();
    }
}
