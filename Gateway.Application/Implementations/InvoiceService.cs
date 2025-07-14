using AutoMapper;
using Gateway.Application.Interfaces;
using Gateway.Application.Invoice.Dtos;
using Gateway.Domain.Interfaces;

namespace Gateway.Application.Implementations;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _repository;
    private readonly IMapper _mapper;

    public InvoiceService(IInvoiceRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<InvoiceDto?> GetByIdAsync(long id)
    {
        var ivoice = await _repository.GetByIdAsync(id);
        return ivoice == null ? null : _mapper.Map<InvoiceDto>(ivoice);
    }

    public async Task<IEnumerable<InvoiceListDto>> GetAllAsync()
    {
        var ivoices = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<InvoiceListDto>>(ivoices);
    }

}