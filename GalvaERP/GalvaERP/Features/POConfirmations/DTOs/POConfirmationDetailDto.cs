using System;
using System.Collections.Generic;

namespace GalvaERP.Features.POConfirmations.DTOs;

public record POConfirmationDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Doku_PO,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_dept,
    string? Kode_Valas,
    double? Kurs,
    string? ContactPr,
    DateTime? Psd,
    DateTime? Etd,
    string? Memo,
    double? Nilai,
    double? PPN,
    double? Diskon,
    string? STS,
    string ETag,
    List<POConfirmationLineDto> Lines);
