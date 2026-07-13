using GalvaERP.Features.Invoices.DTOs;
using MediatR;

namespace GalvaERP.Features.Invoices.Commands;

public record CreatePOBasedAPInvoiceCommand(
    DateTime Tgl,
    string Kode_Supplier,
    string? NOPEN,
    DateTime? TglNopen,
    string? AWB_BL,
    double Amount,
    double PPn,
    double TotalRp,
    string? Keterangan,
    List<InvoicePOLinkDto> POLinks,
    List<InvoiceCostLineDto> CostLines) : IRequest<string>;
