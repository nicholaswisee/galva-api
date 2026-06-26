namespace GalvaERP.Features.Payments.DTOs;

public class PaymentListDto
{
    public string Doku { get; set; } = null!;
    public DateTime? Tgl { get; set; }
    public string? Kode_Supplier { get; set; }
    public string? SupplierName { get; set; }
    public double? NilaiKas { get; set; }
    public double? NilaiGiro { get; set; }
    public string? STS { get; set; }
    public string? ETag { get; set; }
}
