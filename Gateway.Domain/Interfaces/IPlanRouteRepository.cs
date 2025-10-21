using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IPlanRouteRepository
{
    Task<IEnumerable<PlanRoute>> GetAllAsync();
}
