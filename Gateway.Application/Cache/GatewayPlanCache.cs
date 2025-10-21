using Gateway.Application.Contract.Dtos;
using Gateway.Application.Plan.Dtos;

namespace Gateway.Application.Cache;

public class GatewayPlanCache
{
    public List<ContractDto> Contracts { get; set; } = [];
    public List<PlanDto> Plans { get; set; } = [];
    public List<PlanRouteDto> PlanRoutes { get; set; } = [];
}
