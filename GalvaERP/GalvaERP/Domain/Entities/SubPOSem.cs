using System;

namespace GalvaERP.Domain.Entities;

public partial class SubPOSem
{
    public string? kode_BRGganti { get; set; }
    public string? Doku { get; set; }
    public DateTime? Tgl { get; set; }
    public string? Doku_SPPB { get; set; }
    public short? NoUrutSPPB { get; set; }
    public string? Kode_Brg { get; set; }
    public string? Kode_Dept { get; set; }
    public string? Kode_Gudang { get; set; }
    public string? Alias { get; set; }
    public double? HargaJasa { get; set; }
    public double? HargaMaterial { get; set; }
    public double? Harga { get; set; }
    public double? Total { get; set; }
    public string? Kode_Valas { get; set; }
    public double? Diskon { get; set; }
    public double? DiskonTunai { get; set; }
    public double? Jumlah { get; set; }
    public double? JumlahTemp { get; set; }
    public double? JumlahKirim { get; set; }
    public double? JmlKirimTemp { get; set; }
    public double? JumlahVerify { get; set; }
    public double? JumlahVerifyTemp { get; set; }
    public string? Keterangan { get; set; }
    public double? PPN { get; set; }
    public double? PPnBm { get; set; }
    public double? PPH22 { get; set; }
    public double? RTPO { get; set; }
    public double? REALISASI { get; set; }
    public DateTime? TGL_LPB { get; set; }
    public string? KETNPSD { get; set; }
    public double? NilValas { get; set; }
    public string? Doku_LPB { get; set; }
    public string? ExtDokuPO { get; set; }
    public double? SISA_ORDER_TEMP { get; set; }
    public double? REALISASI_TEMP { get; set; }
    public string? TempNama { get; set; }
    public DateTime? TglKirim { get; set; }
    public string? Major { get; set; }
    public string? Ref { get; set; }
    public string? KodeRnd { get; set; }
    public DateTime? EntryDate { get; set; }
    public double? Urut { get; set; }
    public string? UserID { get; set; }
    public double? JumlahKonfirm { get; set; }
    public string? Doku_SO { get; set; }
    public string? KodeRnd_SO { get; set; }
    public long id_sub_posem { get; set; }
    public string? Model { get; set; }
    public string? Merk { get; set; }
    public string? Satuan { get; set; }
    public double? DiscPct { get; set; }
}
