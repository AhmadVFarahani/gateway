using Gateway.Application.Role.Dtos;

namespace Gateway.Application.Interfaces;

public interface IRoleService
{
    Task<RoleDto?> GetByIdAsync(long id);
    Task<IEnumerable<RoleDto>> GetAllAsync();
    Task<long> CreateAsync(CreateRoleRequest request);
    Task UpdateAsync(long id, UpdateRoleRequest request);
    Task DeleteAsync(long id);
}