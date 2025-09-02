using AutoMapper;
using ClosedXML.Excel;
using Gateway.Application.Contract.Dtos;
using Gateway.Application.Interfaces;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class ContractService : IContractService
{
    private readonly IContractRepository _repository;
    private readonly IMapper _mapper;

    public ContractService(IContractRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ContractDto?> GetByIdAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id);
        return user == null ? null : _mapper.Map<ContractDto>(user);
    }

    public async Task<IEnumerable<ContractDto>> GetAllAsync()
    {
        var users = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<ContractDto>>(users);
    }

    public async Task<long> CreateAsync(CreateContractRequest request)
    {

        var role = new Domain.Entities.Contract
        {
            AutoRenew = request.AutoRenew,
            CompanyId = request.CompanyId,
            Description = request.Description,
            EndDate = request.EndDate,
            IsActive = request.IsActive,
            PlanId = request.PlanId,
            StartDate = request.StartDate,

            CreatedAt = DateTime.UtcNow,
        };

        await _repository.AddAsync(role);
        return role.Id;
    }

    public async Task UpdateAsync(long id, UpdateContractRequest request)
    {
        var contract = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Contract not found");

        contract.AutoRenew = request.AutoRenew;
        contract.CompanyId = request.CompanyId;
        contract.Description= request.Description;
        contract.EndDate = request.EndDate;
        contract.IsActive = request.IsActive;
        contract.PlanId = request.PlanId;
        contract.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(contract);
    }

    public async Task DeleteAsync(long id)
    {
        var user = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException("Contract not found");

        await _repository.DeleteAsync(user);
    }

    public async Task<byte[]> ExportToExcel()
    {
        var contarcts = (await _repository.GetAllAsync()).ToList();
        
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Contracts");

        // Header
        worksheet.Cell(1, 1).Value = "CompanyId";
        worksheet.Cell(1, 2).Value = "PlanId";
        worksheet.Cell(1, 3).Value = "StartDate";
        worksheet.Cell(1, 4).Value = "EndDate";
        worksheet.Cell(1, 5).Value = "AutoRenew";
        worksheet.Cell(1, 6).Value = "IsActive";
        worksheet.Cell(1, 7).Value = "Description";

        // Data
        for (int i = 0; i < contarcts.Count; i++)
        {
            var row = i + 2;
            var inv = contarcts[i];

            worksheet.Cell(row, 1).Value = inv.CompanyId;
            worksheet.Cell(row, 2).Value = inv.PlanId;
            worksheet.Cell(row, 3).Value = inv.StartDate;
            worksheet.Cell(row, 4).Value = inv.EndDate;
            worksheet.Cell(row, 5).Value = inv.AutoRenew;
            worksheet.Cell(row, 6).Value = inv.IsActive;
            worksheet.Cell(row, 7).Value = inv.Description;
        }

        // Auto-size columns
        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}