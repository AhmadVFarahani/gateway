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
    Task<IEnumerable<CompanyPlanDto>> GetCompanyPlans(long companyId);
    Task<CompanyPlanDto> GetCompanyPlanById(long companyId, long companyPlanId);
    Task<long> AddPlanToCompany(long companyId,CreateCompanyPlanRequest request);
    Task UpdateCompanyPlan(long companyId, long companyPlanId,  UpdateCompanyPlanRequest request);
    #endregion Plan
}