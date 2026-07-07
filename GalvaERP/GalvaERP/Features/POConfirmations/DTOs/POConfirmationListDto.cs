using System;

namespace GalvaERP.Features.POConfirmations.DTOs;

public class POConfirmationListDto
{
    public string Doku { get; set; } = string.Empty;

    public DateTime? Tgl { get; set; }

    public string? Doku_PO { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? SupplierName { get; set; }

    public double? Nilai { get; set; }

    public string? STS { get; set; }

    public string? ETag { get; set; }
}
