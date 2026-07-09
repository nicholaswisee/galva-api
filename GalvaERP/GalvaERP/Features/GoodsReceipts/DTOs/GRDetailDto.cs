using System;
using System.Collections.Generic;

namespace GalvaERP.Features.GoodsReceipts.DTOs;

public record GRDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Doku_PO,
    string? Doku_PCF,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_Valas,
    double? Kurs,
    string? SuratJalan,
    double? Nilai,
    double? PPN,
    string? STS,
    string? Status,
    string? Memo,
    string ETag,
    List<GRDetailLineDto> LineItems);
