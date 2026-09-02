namespace GalvaERP.Domain.Entities;

public partial class SubReturBeli
{
    public long PKbas { get; set; }
    public string? kode_BRGganti { get; set; }
    public string? Doku { get; set; }
    public DateTime? Tgl { get; set; }
    public string? Kode_Supplier { get; set; }
    public string? Doku_Faktur2 { get; set; }
    public string? NPO { get; set; }
    public string? Kode_Dept { get; set; }
    public string? Kode_Brg { get; set; }
    public string? Kode_Gudang { get; set; }
    public string? Alias { get; set; }
    public double? Jumlah { get; set; }
    public double? Harga { get; set; }
    public double? HPP { get; set; }
    public double? Diskon { get; set; }
    public double? DiskonTunai { get; set; }
    public double? PPN { get; set; }
    public double? PPnBm { get; set; }
    public double? Nilai { get; set; }
    public string? Kode_Valas { get; set; }
    public double? Kurs { get; set; }
    public byte? Comercial { get; set; }
    public short? NoUrut { get; set; }
    public string? UserID { get; set; }
    public string? Hapus { get; set; }
    public DateTime? EntryDate { get; set; }
    public string? KodeRnd { get; set; }
    public string? Doku_Faktur { get; set; }
    public string? Doku_LPB { get; set; }
    public byte[] RowVersion { get; set; } = null!;
}
