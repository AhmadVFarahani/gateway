using AutoMapper;
using Gateway.Application.Invoice.Dtos;

namespace Gateway.Application.Mapping;

public class InvoiceProfile : Profile
{
    public InvoiceProfile()
    {
        CreateMap<Domain.Entities.Invoice, InvoiceDto>().ReverseMap();
        CreateMap<Domain.Entities.Invoice, InvoiceListDto>().ReverseMap();
        CreateMap<Domain.Entities.InvoiceItem, InvoiceItemDto>().ReverseMap();
    }
}
