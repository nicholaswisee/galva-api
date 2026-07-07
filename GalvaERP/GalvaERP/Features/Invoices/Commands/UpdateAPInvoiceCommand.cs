using GalvaERP.Features.Invoices.DTOs;
using MediatR;

namespace GalvaERP.Features.Invoices.Commands;

public record UpdateAPInvoiceCommand(
    string Doku,
    string? STS,
    string? Keterangan,
    string ETag) : IRequest<InvoiceDetailDto>;