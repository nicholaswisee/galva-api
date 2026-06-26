using System;

namespace GalvaERP.Features.PurchaseRequisitions.DTOs;

public record PRDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Kode_Dept,
    string? Status,
    string? NPO,
    string? Kode_Sales,
    double? Total,
    string? Memo,
    string ETag);
