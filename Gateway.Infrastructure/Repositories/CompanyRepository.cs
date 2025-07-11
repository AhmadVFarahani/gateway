﻿using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;
using Gateway.Persistence;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;

namespace Gateway.Infrastructure.Repositories;

public class CompanyRepository : ICompanyRepository
{
    private readonly GatewayDbContext _context;

    public CompanyRepository(GatewayDbContext context)
    {
        _context = context;
    }

    public async Task<Company?> GetByIdAsync(long id) =>
        await _context.Companies.FindAsync(id);
    public async Task<Company?> GetWithPlansAsync(long id) =>
        await _context.Companies
        .Include(c => c.CompanyPlans)
            .ThenInclude(c=>c.Plan)
        .FirstOrDefaultAsync(c => c.Id == id);



    public async Task<Contract?> GetCompanyPlanByIdAsync(long companyId, long companyPlanId) =>
        await _context.Contracts
        .Include(c => c.Plan)
       .FirstOrDefaultAsync(cp => cp.Id == companyPlanId && cp.CompanyId == companyId);


    public async Task<IEnumerable<Company>> GetAllAsync() =>
        await _context.Companies.ToListAsync();

    public async Task AddAsync(Company company)
    {
        _context.Companies.Add(company);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Company company)
    {
        _context.Companies.Update(company);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Company company)
    {
        _context.Companies.Remove(company);
        await _context.SaveChangesAsync();
    }
}