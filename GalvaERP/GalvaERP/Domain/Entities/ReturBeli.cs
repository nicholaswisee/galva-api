namespace GalvaERP.Domain.Entities;

public partial class ReturBeli
{
    public long PKbas { get; set; }
    public string? Doku { get; set; }
    public DateTime? Tgl { get; set; }
    public string? Doku_Faktur { get; set; }
    public string? Kode_Supplier { get; set; }
    public string? Kode_Dept { get; set; }
    public string? Kode_Gudang { get; set; }
    public double? PPn { get; set; }
    public double? PPnTunai { get; set; }
    public double? Diskon { get; set; }
    public double? DiskonTunai { get; set; }
    public double? Total { get; set; }
    public string? Kode_Valas { get; set; }
    public double? Kurs { get; set; }
    public string? STS { get; set; }
    public string? LIHAT { get; set; }
    public double? MATERAI { get; set; }
    public double? NILAI { get; set; }
    public double? NILAI_MUKA { get; set; }
    public double? SYARAT { get; set; }
    public string? AlmKirim { get; set; }
    public string? Type { get; set; }
    public string? UserID { get; set; }
    public string? Hapus { get; set; }
    public DateTime? EntryDate { get; set; }
    public string? StatusGL { get; set; }
    public string? Kode_buyer { get; set; }
    public string? TipeRetur { get; set; }
    public bool? Validasi { get; set; }
    public string? Doku_FP { get; set; }
    public DateTime? Tgl_FP { get; set; }
    public string? EFaktur { get; set; }
    public string? MEMO { get; set; }
    public string? Kode_IDN { get; set; }
    public bool? SyncToCMG { get; set; }
    public bool? CreatedInWMS { get; set; }
    public string? CreatedByInWMS { get; set; }
    public DateTime? CreatedDateInWMS { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}
