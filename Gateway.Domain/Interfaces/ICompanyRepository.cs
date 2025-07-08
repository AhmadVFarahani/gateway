using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(long id);
    Task<Company?> GetWithPlansAsync(long id);
    Task<Company?> GetWithRoutePricingsAsync(long id);
    Task<CompanyPlan?> GetCompanyPlanByIdAsync(long companyId, long companyPlanId);
    Task<CompanyRoutePricing?> GetCompanyRoutePricingByIdAsync(long companyId, long routingPriceId);
    Task<IEnumerable<Company>> GetAllAsync();
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Company company);
}
