using AutoMapper;
using Gateway.Application.Company.Dtos;

namespace Gateway.Application.Mapping;

public class CompanyProfile : Profile
{
    public CompanyProfile()
    {
        CreateMap<Domain.Entities.Company, CompanyDto>().ReverseMap();
    }
}
