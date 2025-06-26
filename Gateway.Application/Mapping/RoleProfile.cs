using AutoMapper;
using Gateway.Application.Role.Dtos;

namespace Gateway.Application.Mapping;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<Domain.Entities.Role, RoleDto>().ReverseMap();
    }
}