using AutoMapper;
using Gateway.Application.Services.Dtos;
using Gateway.Domain.Entities;

namespace Gateway.Application.Mapping;

public class ServiceProfile : Profile
{
    public ServiceProfile()
    {
        CreateMap<Service, ServiceDto>().ReverseMap();
    }
}