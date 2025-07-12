using Gateway.Application.Company.Dtos;
using Gateway.Application.Contract.Dtos;

namespace Gateway.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto?> GetByIdAsync(long id);
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<long> CreateAsync(CreateCompanyRequest request);
    Task UpdateAsync(long id, UpdateCompanyRequest request);
    Task DeleteAsync(long id);

    #region Plan
    Task<IEnumerable<ContractDto>> GetCompanyPlanssAsync(long companyId);
    Task<ContractDto> GetCompanyPlanByIdsAsync(long companyId, long companyPlanId);
    Task<long> AddPlanToCompanysAsync(long companyId,CreateContractRequest request);
    Task UpdateCompanyPlansAsync(long companyId, long companyPlanId,  UpdateContractRequest request);
    #endregion Plan

}