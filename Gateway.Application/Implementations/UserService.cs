using AutoMapper;
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

    public async Task<long> CreateAsync(CreateUserRequest request)
    {
        var passwordHash = HashPassword(request.Password);

        var user = new User
        {
            UserName = request.UserName,
            PasswordHash = passwordHash,
            UserType = request.UserType,
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
}
