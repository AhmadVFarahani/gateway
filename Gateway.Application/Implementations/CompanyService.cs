using AutoMapper;
using Gateway.Application.Company.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Domain.Entities;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class CompanyService : ICompanyService
{
    private readonly ICompanyRepository _repository;
    private readonly IMapper _mapper;

    public CompanyService(ICompanyRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CompanyDto?> GetByIdAsync(long id)
    {
        var company = await _repository.GetByIdAsync(id);
        return company == null ? null : _mapper.Map<CompanyDto>(company);
    }

    public async Task<IEnumerable<CompanyDto>> GetAllAsync()
    {
        var companies = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<CompanyDto>>(companies);
    }

    public async Task<long> CreateAsync(CreateCompanyRequest request)
    {
        var company = new Domain.Entities.Company
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true,
        };

        await _repository.AddAsync(company);
        return company.Id;
    }

    public async Task UpdateAsync(long id, UpdateCompanyRequest request)
    {
        var company = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Service not found");

        company.Name = request.Name;
        company.Description = request.Description;
        company.IsActive = request.IsActive;
        company.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(company);
    }

    public async Task DeleteAsync(long id)
    {
        var company = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Service not found");

        await _repository.DeleteAsync(company);
    }
}