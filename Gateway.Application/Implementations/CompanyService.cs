using AutoMapper;
using ClosedXML.Excel;
using Gateway.Application.Company.Dtos;
using Gateway.Application.Contract.Dtos;
using Gateway.Application.Interfaces;
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

    public async Task<byte[]> ExportToExcel()
    {
        var companies = (await _repository.GetAllAsync()).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Invoices");

        // Header
        worksheet.Cell(1, 1).Value = "Name";
        worksheet.Cell(1, 2).Value = "Description";
        worksheet.Cell(1, 3).Value = "IsActive";
        worksheet.Cell(1, 4).Value = "CreatedAt";

        // Data
        for (int i = 0; i < companies.Count; i++)
        {
            var row = i + 2;
            var inv = companies[i];

            worksheet.Cell(row, 1).Value = inv.Name;
            worksheet.Cell(row, 2).Value = inv.Description;
            worksheet.Cell(row, 3).Value = inv.IsActive;
            worksheet.Cell(row, 4).Value = inv.CreatedAt;
        }

        // Auto-size columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }



    #region CompanyPlans

    public async Task<IEnumerable<ContractDto>> GetCompanyPlanssAsync(long companyId)
    {
        var company = await _repository.GetWithPlansAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");
        return _mapper.Map<IEnumerable<ContractDto>>(company.CompanyPlans);

    }
    public async Task<ContractDto> GetCompanyPlanByIdsAsync(long companyId, long companyPlanId)
    {
        var CompanyPlan = await _repository.GetCompanyPlanByIdAsync(companyId, companyPlanId)
            ?? throw new KeyNotFoundException("Company not found");
        return _mapper.Map<ContractDto>(CompanyPlan);

    }

    public async Task<long> AddPlanToCompanysAsync(long companyId, CreateContractRequest request)
    {
        var company = await _repository.GetWithPlansAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");
        var plan = new Domain.Entities.Contract
        {
            CompanyId = companyId,
            EndDate = request.EndDate,
            StartDate = request.StartDate,
            PlanId = request.PlanId,
            IsActive = request.IsActive,
            AutoRenew = request.AutoRenew,
            CreatedAt = DateTime.UtcNow,
        };

        company.addPlan(plan);
        await _repository.UpdateAsync(company);
        return plan.Id;
    }

    public async Task UpdateCompanyPlansAsync(long companyId, long companyPlanId, UpdateContractRequest request)
    {
        var company = await _repository.GetWithPlansAsync(companyId)
            ?? throw new KeyNotFoundException("Company not found");

        company.updatePlan(companyPlanId, request.StartDate, request.EndDate, request.AutoRenew, request.IsActive);
        await _repository.UpdateAsync(company);
    }


    #endregion
}