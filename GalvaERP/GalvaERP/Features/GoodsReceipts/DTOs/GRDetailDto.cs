namespace GalvaERP.Features.GoodsReceipts.DTOs;

public record GRDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Doku_PO,
    string? Kode_Supplier,
    string? SupplierName,
    string? SuratJalan,
    double? Nilai,
    double? PPN,
    string? STS,
    string? Status,
    string? Memo,
    string ETag);
