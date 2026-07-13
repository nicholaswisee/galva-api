namespace GalvaERP.Features.Invoices.DTOs;

public record InvoiceCostLineDto(
    string? TipeBiaya,
    string? APRef,
    string? InvoiceNo,
    DateTime? TglInvoice,
    short? Term,
    double Amount,
    string? Kode_Valas,
    double Rate,
    double AmountRp,
    string? FakturPajak,
    DateTime? Tgl_FP,
    double PPnPct,
    double PPnRp,
    double TotalRp);
