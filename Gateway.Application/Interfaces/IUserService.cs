﻿using Gateway.Application.Users;

namespace Gateway.Application.Interfaces;

public interface IUserService
{
    Task<UserDto?> GetByIdAsync(long id);
    Task<IEnumerable<UserDto>> GetAllAsync();
    Task<IEnumerable<UserDto>> GetByCompanyId(long companyId);
    Task<long> CreateAsync(CreateUserRequest request);
    Task UpdateAsync(long id, UpdateUserRequest request);
    Task DeleteAsync(long id);
}
