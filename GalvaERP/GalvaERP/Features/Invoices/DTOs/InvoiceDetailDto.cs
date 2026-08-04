using System;
using System.Collections.Generic;

namespace GalvaERP.Features.Invoices.DTOs;

public record InvoiceDetailLineDto(
    long PKbas,
    string? TipeBiaya,
    string? Doku_LPB,
    string? Doku_PO,
    double? NilaiLPB,
    double? Nilai,
    double? PPn,
    string? APRef,
    string? InvoiceNo,
    DateTime? TglInvoice,
    string? Doku_FP);

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
    string? TipeBiaya,
    string ETag,
    List<InvoiceDetailLineDto> LineItems);