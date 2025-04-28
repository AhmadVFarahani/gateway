using AutoMapper;
using Gateway.Application.Users;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>().ReverseMap();
    }
}