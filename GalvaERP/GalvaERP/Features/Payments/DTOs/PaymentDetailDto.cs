namespace GalvaERP.Features.Payments.DTOs;

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
    string ETag);
