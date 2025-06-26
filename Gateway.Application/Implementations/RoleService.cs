using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Role.Dtos;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _repository;
    private readonly IMapper _mapper;

    public RoleService(IRoleRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<RoleDto?> GetByIdAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<RoleDto>(user);
    }

    public async Task<IEnumerable<RoleDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(users);
    }

    public async Task<long> CreateAsync(CreateRoleRequest request)
    {

       var role = new Domain.Entities.Role
        {
            Name = request.Name,
            Description= request.Description,
            CreatedAt = DateTime.UtcNow,
       };

        await _repository.AddAsync(role);
        return role.Id;
    }

    public async Task UpdateAsync(long id, UpdateRoleRequest request)
    {
        var role = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Role not found");

        role.Name = request.Name;
        role.Description = request.Description;
        role.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(role);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Role not found");

        await _repository.DeleteAsync(user);
    }
}