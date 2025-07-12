using AutoMapper;
using Gateway.Application.Contract.Dtos;

namespace Gateway.Application.Mapping;

public class ContractProfile : Profile
{
    public ContractProfile()
    {
        CreateMap<Domain.Entities.Contract, ContractDto>().ReverseMap();
    }
}
