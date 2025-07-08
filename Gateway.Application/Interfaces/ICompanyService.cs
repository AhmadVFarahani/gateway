using Gateway.Application.Company.Dtos;

namespace Gateway.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto?> GetByIdAsync(long id);
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<long> CreateAsync(CreateCompanyRequest request);
    Task UpdateAsync(long id, UpdateCompanyRequest request);
    Task DeleteAsync(long id);

    #region Plan
    Task<IEnumerable<CompanyPlanDto>> GetCompanyPlanssAsync(long companyId);
    Task<CompanyPlanDto> GetCompanyPlanByIdsAsync(long companyId, long companyPlanId);
    Task<long> AddPlanToCompanysAsync(long companyId,CreateCompanyPlanRequest request);
    Task UpdateCompanyPlansAsync(long companyId, long companyPlanId,  UpdateCompanyPlanRequest request);
    #endregion Plan

    #region RoutePricing
    Task<IEnumerable<CompanyRoutePricingDto>> GetCompanyRoutePricingssAsync(long companyId);
    Task<CompanyRoutePricingDto> GetCompanyRoutePricingByIdsAsync(long companyId, long routingPriceId);
    Task<long> AddRoutePricingToCompanysAsync(long companyId, CreateCompanyRoutePricingRequest request);
    Task UpdateCompanyRoutePricingAsync(long companyId, long routingPriceId, UpdateCompanyRoutePricingRequest request);
    Task DeleteRoutePricingAsync(long companyId, long routingPriceId);
    #endregion RoutePricing
}