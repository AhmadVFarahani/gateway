using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Plan.Dtos;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class PlanService : IPlanService
{
    private readonly IPlanRepository _repository;
    private readonly IMapper _mapper;

    public PlanService(IPlanRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PlanDto?> GetByIdAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<PlanDto>(user);
    }

    public async Task<IEnumerable<PlanDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<PlanDto>>(users);
    }

    public async Task<long> CreateAsync(CreatePlanRequest request)
    {

       var plan = new Domain.Entities.Plan
        {
            Name = request.Name,
            Description= request.Description,

            IsUnlimited = request.IsUnlimited,
            MonthlyPrice = request.MonthlyPrice,    
            MaxRequestsPerMonth = request.MaxRequestsPerMonth,

           CreatedAt = DateTime.UtcNow,
       };

        await _repository.AddAsync(plan);
        return plan.Id;
    }

    public async Task UpdateAsync(long id, UpdatePlanRequest request)
    {
        var plan = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Plan not found");

        plan.Name = request.Name;
        plan.Description = request.Description;

        plan.IsUnlimited = request.IsUnlimited;
        plan.MonthlyPrice = request.MonthlyPrice;
        plan.MaxRequestsPerMonth = request.MaxRequestsPerMonth;

        plan.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(plan);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Plan not found");

        await _repository.DeleteAsync(user);
    }
}