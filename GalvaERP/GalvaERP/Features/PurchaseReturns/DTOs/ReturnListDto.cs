namespace GalvaERP.Features.PurchaseReturns.DTOs;

public sealed class ReturnListDto
{
    public string Doku { get; init; } = string.Empty;
    public DateTime? Tgl { get; init; }
    public string? Doku_Faktur { get; init; }
    public string? Kode_Supplier { get; init; }
    public string? SupplierName { get; init; }
    public string? Kode_Valas { get; init; }
    public double Nilai { get; init; }
    public string STS { get; init; } = string.Empty;
    public bool SyncToCMG { get; init; }
    public string ETag { get; init; } = string.Empty;
}
