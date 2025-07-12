using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Contract.Dtos;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class ContractService : IContractService
{
    private readonly IContractRepository _repository;
    private readonly IMapper _mapper;

    public ContractService(IContractRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ContractDto?> GetByIdAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<ContractDto>(user);
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ContractDto>>(users);
    }

    public async Task<long> CreateAsync(CreateContractRequest request)
    {

        var role = new Domain.Entities.Contract
        {
            AutoRenew = request.AutoRenew,
            CompanyId = request.CompanyId,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            PlanId = request.PlanId,
            StartDate = request.StartDate,

            CreatedAt = DateTime.UtcNow,
        };

        await _repository.AddAsync(role);
        return role.Id;
    }

    public async Task UpdateAsync(long id, UpdateContractRequest request)
    {
        var contract = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Contract not found");

        contract.AutoRenew = request.AutoRenew;
        contract.CompanyId = request.CompanyId;
        contract.EndDate = request.EndDate;
        contract.IsActive = request.IsActive;
        contract.PlanId = request.PlanId;
        contract.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(contract);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Contract not found");

        await _repository.DeleteAsync(user);
    }
}