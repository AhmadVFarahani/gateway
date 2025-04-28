using Gateway.Domain.Entities;

namespace Gateway.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(long id);
    Task<IEnumerable<User>> GetAllAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
}