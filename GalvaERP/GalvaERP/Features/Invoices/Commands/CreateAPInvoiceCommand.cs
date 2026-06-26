using GalvaERP.Features.Invoices.DTOs;
using MediatR;

namespace GalvaERP.Features.Invoices.Commands;

public record CreateAPInvoiceCommand(
    string Kode_Supplier,
    DateTime Tgl,
    string? Kode_Dept,
    string? Kode_Bank,
    double Nilai,
    double PPn,
    double Diskon,
    double Misc,
    string? Keterangan,
    List<InvoiceGRLinkDto> GRLinks) : IRequest<string>;
