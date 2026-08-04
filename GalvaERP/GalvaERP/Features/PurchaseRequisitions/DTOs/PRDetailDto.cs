using System;
using System.Collections.Generic;

namespace GalvaERP.Features.PurchaseRequisitions.DTOs;

public record PRDetailLineDto(
    long id_sub_spb,
    string? Kode_Brg,
    double? Jumlah,
    double? Harga,
    double? Nilai,
    string? Kode_Gudang,
    string? Alias);

public record PRDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Kode_Dept,
    string? Status,
    string? NPO,
    string? Kode_Sales,
    double? Total,
    string? Memo,
    bool? StsVerify,
    DateTime? TglVerify,
    string ETag,
    List<PRDetailLineDto> LineItems);
