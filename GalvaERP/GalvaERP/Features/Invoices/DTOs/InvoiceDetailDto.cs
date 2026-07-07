namespace GalvaERP.Features.Invoices.DTOs;

public record InvoiceDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_Dept,
    double? Nilai,
    double? PPn,
    double? Diskon,
    double? Misc,
    string? STS,
    string? Keterangan,
    string ETag);