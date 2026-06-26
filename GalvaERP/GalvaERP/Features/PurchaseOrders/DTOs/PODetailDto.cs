using System;

namespace GalvaERP.Features.PurchaseOrders.DTOs;

public record PODetailDto(
    string Doku,
    DateTime? Tgl,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_dept,
    double? Nilai,
    double? PPN,
    double? Diskon,
    string? STS,
    string? Memo,
    string ETag);
