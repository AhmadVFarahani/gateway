using AutoMapper;
using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Application.Plan.Dtos;
using Gateway.Domain.Entities;
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



    #region Routes

    public async Task<IEnumerable<PlanRouteDto>> GetPlanRouteAsync(long planId)
    {
        var plan = await _planRepository.GetWithRouteAsync(planId)
            ?? throw new KeyNotFoundException("Plan not found");
        return _mapper.Map<IEnumerable<PlanRouteDto>>(plan.PlanRoutes);
    }

    public async Task<PlanRouteDto> GetPlanRouteByIdAsync(long planId, long planRouteId)
    {
        var planRoute = await _planRepository.GetPlanRouteByIdAsync(planId, planRouteId)
              ?? throw new KeyNotFoundException("PlanRoute not found");
        return _mapper.Map<PlanRouteDto>(planRoute);
    }

    public async Task<long> AddRouteToPlanAsync(long planId, CreatePlanRouteRequest request)
    {
        var plan = await _planRepository.GetWithRouteAsync(planId)
             ?? throw new KeyNotFoundException("Plan not found");
        var planRoute = new PlanRoute
        {
            IsFree = request.IsFree,
            FlatPricePerCall = request.FlatPricePerCall,
            TieredPricingJson = request.TieredPricingJson,
            PlanId = planId,
            RouteId = request.RouteId,

            CreatedAt = DateTime.UtcNow,
        };

        plan.addRoute(planRoute);
        await _planRepository.UpdateAsync(plan);
        return plan.Id;
    }

    public async Task UpdatePlanRouteAsync(long planId, long planRouteId, UpdatePlanRouteRequest request)
    {
        var plan = await _planRepository.GetWithRouteAsync(request.PlanId)
           ?? throw new KeyNotFoundException("Plan not found");

        plan.UpdateRoute(planId, request.FlatPricePerCall, request.TieredPricingJson, request.IsFree);

        await _planRepository.UpdateAsync(plan);
    }

    public async Task DeleteRoutePlanAsync(long planId, long planRouteId)
    {
        var plan = await _planRepository.GetWithRouteAsync(planId)
             ?? throw new KeyNotFoundException("Plan not found");

        plan.deleteRoute(planRouteId);
        await _planRepository.UpdateAsync(plan);
    }
    #endregion Routes
}