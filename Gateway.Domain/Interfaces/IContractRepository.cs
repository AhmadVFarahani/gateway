using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IContractRepository
{
    Task<Contract?> GetByIdAsync(long id);
    Task<IEnumerable<Contract>> GetAllAsync();
    Task AddAsync(Contract role);
    Task UpdateAsync(Contract role);
    Task DeleteAsync(Contract role);
}