﻿using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gateway.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly GatewayDbContext _context;

    public UserRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(long id)
    {
        return await _context.Users.Include(c => c.Company).Include(c => c.Role).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<User?> GetWithAccessPolicyAsync(long id) =>
        await _context.Users
        .Include(c => c.AccessPolicies)
            .ThenInclude(c=>c.Scope)
        .Include(c => c.AccessPolicies)
            .ThenInclude(c => c.ApiKey)
        //.Include(c=>c.ApiKeys)
        .FirstOrDefaultAsync(c => c.Id == id);
    public async Task<AccessPolicy?> GetAccessPolicyByIdAsync(long userId, long accessPolicyId) =>
        await _context.AccessPolicies
        .Include(c => c.User)
       .FirstOrDefaultAsync(cp => cp.Id == accessPolicyId && cp.UserId == userId);

    public async Task<User?> GetWithApiKeyAsync(long id) =>
       await _context.Users
       .Include(c => c.ApiKeys)
       .FirstOrDefaultAsync(c => c.Id == id);
    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return await _context.Users.Include(c => c.Company).Include(c => c.Role).ToListAsync();
    }

    public async Task<IEnumerable<User>> GetByCompanyId(long companyId)
    {
        return await _context.Users.Include(c => c.Company).Include(c => c.Role).Where(c=>c.CompanyId==companyId).ToListAsync();
    }

    public async Task AddAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(User user)
    {
        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
    }
}
