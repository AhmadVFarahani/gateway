using Gateway.Application.Routes.Dtos;
using Gateway.Application.Services.Dtos;

namespace Gateway.Application.Cache;

public class YarpRoteConfigCache
{
    public List<ServiceDto> Services { get; set; } = [];
    public List<RouteDto> Routes { get; set; } = [];
}
