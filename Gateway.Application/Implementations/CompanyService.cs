using AutoMapper;
using Gateway.Application.Company.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly IMapper _mapper;

    public CompanyService(ICompanyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto?> GetByIdAsync(long id)
    {
        var company = await _repository.GetByIdAsync(id);
        return company == null ? null : _mapper.Map<CompanyDto>(company);
    }

    public async Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        var companies = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<long> CreateAsync(CreateCompanyRequest request)
    {
        var company = new Domain.Entities.Company
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
        };

        await _repository.AddAsync(company);
        return company.Id;
    }

    public async Task UpdateAsync(long id, UpdateCompanyRequest request)
    {
        var company = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Service not found");

        company.Name = request.Name;
        company.Description = request.Description;
        company.IsActive = request.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(company);
    }

    public async Task DeleteAsync(long id)
    {
        var company = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Service not found");

        await _repository.DeleteAsync(company);
    }





    #region CompanyPlans

    public async Task<IEnumerable<CompanyPlanDto>> GetCompanyPlanssAsync(long companyId)
    {
        var company = await _repository.GetWithPlansAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");
        return _mapper.Map<IEnumerable<CompanyPlanDto>>(company.CompanyPlans);

    }
    public async Task<CompanyPlanDto> GetCompanyPlanByIdsAsync(long companyId, long companyPlanId)
    {
        var CompanyPlan = await _repository.GetCompanyPlanByIdAsync(companyId, companyPlanId)
            ?? throw new KeyNotFoundException("Company not found");
        return _mapper.Map<CompanyPlanDto>(CompanyPlan);

    }

    public async Task<long> AddPlanToCompanysAsync(long companyId, CreateCompanyPlanRequest request)
    {
        var company = await _repository.GetWithPlansAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");
        var plan = new Domain.Entities.CompanyPlan
        {
            CompanyId = companyId,
            EndDate = request.EndDate,
            StartDate = request.StartDate,
            PlanId = request.PlanId,
            IsActive = request.IsActive,
            AutoRenew = request.AutoRenew,
            CreatedAt = DateTime.UtcNow,
        };

        company.addPlan(plan);
        await _repository.UpdateAsync(company);
        return plan.Id;
    }

    public async Task UpdateCompanyPlansAsync(long companyId, long companyPlanId, UpdateCompanyPlanRequest request)
    {
        var company = await _repository.GetWithPlansAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");

        company.updatePlan(companyPlanId, request.StartDate, request.EndDate, request.AutoRenew, request.IsActive);
        await _repository.UpdateAsync(company);
    }


    #endregion

    #region RoutePricing

    public async Task<IEnumerable<CompanyRoutePricingDto>> GetCompanyRoutePricingssAsync(long companyId)
    {
        var company = await _repository.GetWithRoutePricingsAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");
        return _mapper.Map<IEnumerable<CompanyRoutePricingDto>>(company.CompanyPlans);

    }
    public async Task<CompanyRoutePricingDto> GetCompanyRoutePricingByIdsAsync(long companyId, long routingPriceId)
    {
        var CompanyPlan = await _repository.GetCompanyRoutePricingByIdAsync(companyId, routingPriceId)
            ?? throw new KeyNotFoundException("Company not found");
        return _mapper.Map<CompanyRoutePricingDto>(CompanyPlan);

    }

    public async Task<long> AddRoutePricingToCompanysAsync(long companyId, CreateCompanyRoutePricingRequest request)
    {
        var company = await _repository.GetWithRoutePricingsAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");
        var routePricing = new Domain.Entities.CompanyRoutePricing
        {
            CompanyId = companyId,
            BillingType = request.BillingType,
            RouteId = request.RouteId,
            MaxFreeCalls = request.MaxFreeCalls,
            MaxFreeCallsPerMonth = request.MaxFreeCallsPerMonth,
            PricePerCall = request.PricePerCall,
            MonthlySubscriptionPrice = request.MonthlySubscriptionPrice,
            TieredJson = request.TieredJson,

            IsActive = request.IsActive,
            CreatedAt = DateTime.UtcNow,
        };

        company.addRoutePricing(routePricing);
        await _repository.UpdateAsync(company);
        return routePricing.Id;
    }

    public async Task UpdateCompanyRoutePricingAsync(long companyId, long routingPriceId, UpdateCompanyRoutePricingRequest request)
    {
        var company = await _repository.GetWithRoutePricingsAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");

        company.updateRoutePricing(routingPriceId, request.BillingType,request.RouteId,request.PricePerCall,request.MaxFreeCallsPerMonth,request.MaxFreeCalls,request.TieredJson,request.MonthlySubscriptionPrice,request.IsActive);
        await _repository.UpdateAsync(company);
    }

    public async Task DeleteRoutePricingAsync(long companyId, long routingPriceId)
    {
        var company = await _repository.GetWithRoutePricingsAsync(companyId)
            ?? throw new KeyNotFoundException("Scope not found");

        company.deleteRoutePricing(routingPriceId);
        await _repository.UpdateAsync(company);
    }
    #endregion
}