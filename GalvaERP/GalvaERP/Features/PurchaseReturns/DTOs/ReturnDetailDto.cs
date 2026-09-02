using System.Text.Json.Serialization;

namespace GalvaERP.Features.PurchaseReturns.DTOs;

public record ReturnDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Doku_Faktur,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_Dept,
    string? Kode_Valas,
    double Kurs,
    [property: JsonPropertyName("ppn")] double PPn,
    double Diskon,
    double Total,
    double Nilai,
    string? Memo,
    string STS,
    string? StatusGL,
    bool Validasi,
    bool SyncToCMG,
    bool CreatedInWMS,
    string? Type,
    string? TipeRetur,
    string? Doku_FP,
    DateTime? Tgl_FP,
    string ETag,
    List<ReturnDetailLineDto> LineItems);
