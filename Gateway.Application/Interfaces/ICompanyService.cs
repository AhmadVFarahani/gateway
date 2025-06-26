using Gateway.Application.Company.Dtos;

namespace Gateway.Application.Interfaces;

public interface ICompanyService
{
    Task<CompanyDto?> GetByIdAsync(long id);
    Task<IEnumerable<CompanyDto>> GetAllAsync();
    Task<long> CreateAsync(CreateCompanyRequest request);
    Task UpdateAsync(long id, UpdateCompanyRequest request);
    Task DeleteAsync(long id);
}