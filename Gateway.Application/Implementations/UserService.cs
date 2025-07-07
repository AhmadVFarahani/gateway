using AutoMapper;
using Gateway.Application.AccessPolicies.Dtos;
using Gateway.Application.ApiKeys;
using Gateway.Application.Interfaces;
using Gateway.Application.Users;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Gateway.Application.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _repository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<UserDto?> GetByIdAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<IEnumerable<UserDto>> GetByCompanyId(long companyId)
    {
        var users = await _repository.GetByCompanyId(companyId);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<long> CreateAsync(CreateUserRequest request)
    {
        var passwordHash = HashPassword(request.Password);

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            UserType = request.UserType,
            CompanyId = request.CompanyId,
            RoleId = request.RoleId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _repository.AddAsync(user);
        return user.Id;
    }

    public async Task UpdateAsync(long id, UpdateUserRequest request)
    {
        var user = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        user.UserName = request.UserName;
        user.UserType = request.UserType;
        user.IsActive = request.IsActive;
        user.CompanyId = request.CompanyId;
        user.RoleId = request.RoleId;


        user.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(user);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("User not found");

        await _repository.DeleteAsync(user);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }


    #region AccessPolicies
    public async Task<IEnumerable<AccessPolicyDto>> GetAccessPoliciesAsync(long userId)
    {
        var user = await _repository.GetWithAccessPolicyAsync(userId)
            ?? throw new KeyNotFoundException("User not found");
        return _mapper.Map<IEnumerable<AccessPolicyDto>>(user.AccessPolicies);
    }
    public async Task<AccessPolicyDto> GetAccessPolicyByIdAsync(long userId, long accessPolicyId)
    {
        var accessPolicy = await _repository.GetAccessPolicyByIdAsync(userId, accessPolicyId)
            ?? throw new KeyNotFoundException("User not found");
        return _mapper.Map<AccessPolicyDto>(accessPolicy);
    }
    public async Task<long> AddAccessPolicyToUserAsync(long userId, CreateAccessPolicyRequest request)
    {
        var user = await _repository.GetWithAccessPolicyAsync(userId)
           ?? throw new KeyNotFoundException("User not found");
        var plan = new Domain.Entities.AccessPolicy
        {
            ApiKeyId = request.ApiKeyId,
            IsActive = true,
            ScopeId = request.ScopeId,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
        };

        user.addAccessPolicy(plan);
        await _repository.UpdateAsync(user);
        return plan.Id;
    }

    public async Task DeleteAccessPolicyAsync(long userId, long accessPolicyId)
    {
        var user = await _repository.GetWithAccessPolicyAsync(userId)
             ?? throw new KeyNotFoundException("User not found");

        user.deleteAccessPolicy(accessPolicyId);
        await _repository.UpdateAsync(user);
    }
    //public async Task UpdateCompanyPlan(long companyId, long companyPlanId, UpdateAccessPolicyRequest request)
    //{
    //    throw new NotImplementedException();
    //}


    #endregion AccessPolicies

    #region APIKeys
    public async Task<IEnumerable<ApiKeyDto>> GetApiKeysAsync(long userId)
    {
        var user = await _repository.GetWithApiKeyAsync(userId)
            ?? throw new KeyNotFoundException("User not found");
        return _mapper.Map<IEnumerable<ApiKeyDto>>(user.ApiKeys);
    }
    #endregion APIKeys


}
