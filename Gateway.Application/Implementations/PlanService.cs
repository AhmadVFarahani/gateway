using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Plan.Dtos;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _planRepository;
    private readonly IRouteRepository _routeRepository;
    private readonly IMapper _mapper;

    public PlanService(IPlanRepository repository, IMapper mapper, IRouteRepository routeRepository)
    {
        _planRepository = repository;
        _mapper = mapper;
        _routeRepository = routeRepository;
    }

    public async Task<PlanDto?> GetByIdAsync(long id)
    {
        var plan = await _planRepository.GetByIdAsync(id);
        return plan == null ? null : _mapper.Map<PlanDto>(plan);
    }

    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var plans = await _planRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<PlanDto>>(plans);
    }

    public async Task<long> CreateAsync(CreatePlanRequest request)
    {

        var plan = new Domain.Entities.Plan
        {
            Name = request.Name,
            Description = request.Description,

            IsUnlimited = request.IsUnlimited,
            MonthlyPrice = request.MonthlyPrice,
            MaxRequestsPerMonth = request.MaxRequestsPerMonth,

            CreatedAt = DateTime.UtcNow,
        };

        await _planRepository.AddAsync(plan);
        return plan.Id;
    }

    public async Task UpdateAsync(long id, UpdatePlanRequest request)
    {
        var plan = await _planRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Plan not found");

        plan.Name = request.Name;
        plan.Description = request.Description;

        plan.IsUnlimited = request.IsUnlimited;
        plan.MonthlyPrice = request.MonthlyPrice;
        plan.MaxRequestsPerMonth = request.MaxRequestsPerMonth;

        plan.UpdatedAt = DateTime.UtcNow;

        await _planRepository.UpdateAsync(plan);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _planRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Plan not found");

        await _planRepository.DeleteAsync(user);
    }


    #region PlanCompanies


    #endregion



    #region PlanRoute
    public async Task<PlanRouteDto?> GetPlanRouteByIdAsync(long id)
    {
        var plan = await _planRepository.GetPlanRouteByIdAsync(id);
        return plan == null ? null : _mapper.Map<PlanRouteDto>(plan);
    }

    public async Task<IEnumerable<PlanRouteDto>> GetPlanRouteByPlanId(long id)
    {
        var planRoute = await _planRepository.GetPlanRouteByPlanId(id);
        return _mapper.Map<IEnumerable<PlanRouteDto>>(planRoute);
    }
    public async Task<IEnumerable<PlanRouteDto>> GetPlanRouteByRouteId(long id)
    {
        var planRoute = await _planRepository.GetPlanRouteByRouteId(id);
        var response = _mapper.Map<IEnumerable<PlanRouteDto>>(planRoute);

        return response;
    }

    public async Task<long> CreatePlanRouteAsync(CreatePlanRouteRequest request)
    {
        var plan = await _planRepository.GetByIdAsync(request.PlanId)
            ?? throw new KeyNotFoundException("Plan not found");
        var route = await _routeRepository.GetByIdAsync(request.RouteId)
            ?? throw new KeyNotFoundException("Route not found");

        var planRoute = new Domain.Entities.PlanRoute
        {
            PlanId = request.PlanId,
            RouteId = request.RouteId,
            FlatPricePerCall = request.FlatPricePerCall,
            TieredPricingJson = request.TieredPricingJson,
            IsFree = request.IsFree,

            CreatedAt = DateTime.UtcNow,
        };

        plan.addRoute(planRoute);

        await _planRepository.UpdateAsync(plan);
        return planRoute.Id;
    }

    public async Task UpdatePlanRouteAsync(long id, UpdatePlanRouteRequest request)
    {
        var plan = await _planRepository.GetByIdAsync(request.PlanId)
            ?? throw new KeyNotFoundException("Plan not found");

        plan.UpdateRoute(id, request.FlatPricePerCall, request.TieredPricingJson, request.IsFree);

        await _planRepository.UpdateAsync(plan);
    }

    #endregion
}