using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class LPB
{
    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public DateTime? Tgl_Ganti { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Doku_PO { get; set; }

    public string? Doku_PCF { get; set; }

    public string? SuratJalan { get; set; }

    public DateTime? TglSuratJalan { get; set; }

    public DateTime? TglCreate { get; set; }

    public DateTime? Tgl_Pembayaran { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? PPN { get; set; }

    public double? PPnTunai { get; set; }

    public double? PPnBm { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public double? Nilai { get; set; }

    public string? ForwardAgent { get; set; }

    public string? Memo { get; set; }

    public string? STP { get; set; }

    public short? JMRLPB { get; set; }

    public string? MOBIL { get; set; }

    public string? STS { get; set; }

    public string? Ext_Doku_PO { get; set; }

    public string? Status { get; set; }

    public bool? Status_Edit { get; set; }

    public string? rptUserId { get; set; }

    public string? StatusGL { get; set; }

    public string? Kode_Sup_Biaya_Asuransi { get; set; }

    public string? Kode_Valas_Asuransi { get; set; }

    public double? KursAsuransi { get; set; }

    public double? Biaya_Asuransi { get; set; }

    public string? Kode_Sup_Biaya_Interest { get; set; }

    public string? Kode_Valas_Interest { get; set; }

    public double? KursInterest { get; set; }

    public double? Biaya_Interest { get; set; }

    public string? Kode_Sup_Biaya_Exp1 { get; set; }

    public string? Kode_Valas_Exp1 { get; set; }

    public double? KursExp1 { get; set; }

    public double? Biaya_Exp1 { get; set; }

    public string? Kode_Sup_Biaya_Exp2 { get; set; }

    public string? Kode_Valas_Exp2 { get; set; }

    public double? KursExp2 { get; set; }

    public double? Biaya_Exp2 { get; set; }

    public string? Kode_Sup_Biaya_Angkut { get; set; }

    public string? Kode_Valas_Angkut { get; set; }

    public double? KursAngkut { get; set; }

    public double? Biaya_Angkut { get; set; }

    public string? Kode_Sup_Biaya_LC { get; set; }

    public string? Kode_Valas_LC { get; set; }

    public double? KursLC { get; set; }

    public double? Biaya_LC { get; set; }

    public string? Kode_Sup_Biaya_Bea { get; set; }

    public string? Kode_Valas_Bea { get; set; }

    public double? KursBea { get; set; }

    public double? Biaya_Bea { get; set; }

    public string? Kode_Sup_Biaya_Lain { get; set; }

    public string? Kode_Valas_Lain { get; set; }

    public double? KursLain { get; set; }

    public double? Biaya_Lain { get; set; }

    public string? STS_Biaya { get; set; }

    public short? Term { get; set; }

    public short? Syarat { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public bool? Validasi { get; set; }

    public string? Kode_buyer { get; set; }

    public double? BiayaMasuk { get; set; }

    public double? BiayaMasukP { get; set; }

    public long id_lpb { get; set; }

    public string? Kode_IDN { get; set; }

    public string? ModulSource { get; set; }

    public bool? SyncToCMG { get; set; }

    public bool? CreatedInWMS { get; set; }

    public string? CreatedByInWMS { get; set; }

    public DateTime? CreatedDateInWMS { get; set; }

    public double? DPPNilaiLain { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
