using GalvaERP.Features.Invoices.DTOs;
using MediatR;

namespace GalvaERP.Features.Invoices.Commands;

public record UpdateAPInvoiceCommand(
    string Doku,
    string? STS,
    string? Status,
    string? Keterangan,
    string? Kode_Bank,
    string ETag) : IRequest<InvoiceDetailDto>;
