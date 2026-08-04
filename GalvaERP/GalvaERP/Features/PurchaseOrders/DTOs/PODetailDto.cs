using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace GalvaERP.Features.PurchaseOrders.DTOs;

public record PODetailDto(
    string Doku,
    DateTime? Tgl,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_dept,
    string? Kode_Valas,
    double? Kurs,
    double? Nilai,
    double? DppNilaiLain,
    double? PPN,
    [property: JsonPropertyName("ppnTunai")] double? PPnTunai,
    double? Diskon,
    short? Syarat,
    string? STS,
    string? Memo,
    bool? StsVerify,
    DateTime? TglVerify,
    string ETag,
    List<PODetailLineDto> Lines);
