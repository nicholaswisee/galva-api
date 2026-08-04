using System;

namespace GalvaERP.Features.PurchaseOrders.DTOs;

public class POListDto
{
    public string Doku { get; set; } = string.Empty;
    public DateTime? Tgl { get; set; }
    public string? Kode_Supplier { get; set; }
    public string? SupplierName { get; set; }
    public double? Nilai { get; set; }
    public string? STS { get; set; }
    public bool? StsVerify { get; set; }
    public string? ETag { get; set; }
}
