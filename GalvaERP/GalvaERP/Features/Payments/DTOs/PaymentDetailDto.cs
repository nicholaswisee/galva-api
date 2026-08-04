using System;
using System.Collections.Generic;

namespace GalvaERP.Features.Payments.DTOs;

public record PaymentDetailLineDto(
    long PKbas,
    string? Doku_Faktur,
    string? Doku_LPB,
    double? Nilai,
    double? TotalNilai,
    double? DiskonTunai,
    string? Keterangan);

public record PaymentDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_BankSupplier,
    string? Keterangan,
    double? NilaiKas,
    double? NilaiGiro,
    double? NilMuka,
    string? STS,
    string? Kode_Valas,
    double? Kurs,
    string ETag,
    List<PaymentDetailLineDto> LineItems);
