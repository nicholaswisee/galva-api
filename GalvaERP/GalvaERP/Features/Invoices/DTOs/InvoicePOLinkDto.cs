namespace GalvaERP.Features.Invoices.DTOs;

public record InvoicePOLinkDto(
    string Doku_PO,
    DateTime? Tgl,
    double Amount,
    double Tax,
    string? BasedOn);
