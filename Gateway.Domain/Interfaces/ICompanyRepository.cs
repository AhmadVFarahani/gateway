using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface ICompanyRepository
{
    Task<Company?> GetByIdAsync(long id);
    Task<IEnumerable<Company>> GetAllAsync();
    Task AddAsync(Company company);
    Task UpdateAsync(Company company);
    Task DeleteAsync(Company company);
}
