namespace GalvaERP.Features.GoodsReceipts.DTOs;

public class GRListDto
{
    public string Doku { get; set; } = null!;
    public DateTime? Tgl { get; set; }
    public string? Doku_PO { get; set; }
    public string? Kode_Supplier { get; set; }
    public string? SupplierName { get; set; }
    public double? Nilai { get; set; }
    public string? STS { get; set; }
    public string? Status { get; set; }
    public string? ETag { get; set; }
}
