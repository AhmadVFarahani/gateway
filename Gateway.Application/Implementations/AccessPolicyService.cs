using AutoMapper;
using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class AccessPolicyService : IAccessPolicyService
{
    private readonly IAccessPolicyRepository _repository;
    private readonly IMapper _mapper;

    public AccessPolicyService(IAccessPolicyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<AccessPolicyDto>> GetAllAsync()
    {
        var policies = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<AccessPolicyDto>>(policies);
    }

    public async Task<long> CreateAsync(CreateAccessPolicyRequest request)
    {
        if (request.UserId == null && request.ApiKeyId == null)
            throw new ArgumentException("Either UserId or ApiKeyId must be provided.");

        var policy = new AccessPolicy
        {
            UserId = request.UserId,
            ApiKeyId = request.ApiKeyId,
            ScopeId = request.ScopeId,
            IsActive = true,
        };

        await _repository.AddAsync(policy);
        return policy.Id;
    }

    public async Task DeleteAsync(long id)
    {
        var policy = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Access policy not found.");

        await _repository.DeleteAsync(policy);
    }
}