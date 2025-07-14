using Gateway.Application.Contract.Dtos;

namespace Gateway.Application.Interfaces;

public interface IContractService
{
    Task<ContractDto?> GetByIdAsync(long id);
    Task<IEnumerable<ContractDto>> GetAllAsync();
    Task<long> CreateAsync(CreateContractRequest request);
    Task UpdateAsync(long id, UpdateContractRequest request);
    Task DeleteAsync(long id);
}
