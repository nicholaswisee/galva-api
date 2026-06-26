using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Faktur
{
    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Doku_SJ { get; set; }

    public string? Doku_SPB { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Kode_SubCustomer { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Kode_Gudang { get; set; }

    public string? Destination { get; set; }

    public DateTime? Tgl_ETD { get; set; }

    public DateTime? TGL_ETA { get; set; }

    public DateTime? TGL_PACKING { get; set; }

    public DateTime? TGL_SPB { get; set; }

    public string? ETD { get; set; }

    public string? ETA { get; set; }

    public string? LOADING { get; set; }

    public string? VESSEL1 { get; set; }

    public string? VESSEL2 { get; set; }

    public string? MOS { get; set; }

    public string? HUBUNGI { get; set; }

    public string? NPO { get; set; }

    public double? PPn { get; set; }

    public double? PPnTunai { get; set; }

    public double? PPnBm { get; set; }

    public double? PPnBmTunai { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? TOTAL { get; set; }

    public string? KODE_VALAS { get; set; }

    public double? Kurs { get; set; }

    public double? KursPajak { get; set; }

    public string? SHIP { get; set; }

    public string? PAYMENT { get; set; }

    public string? NAMAUSER { get; set; }

    public double? JMPRN { get; set; }

    public string? STS { get; set; }

    public string? Status { get; set; }

    public DateTime? WAKTU { get; set; }

    public string? LIHAT { get; set; }

    public double? MATERAI { get; set; }

    public double? NILAI { get; set; }

    public double? NILAI_MUKA { get; set; }

    public string? Kode_Sales { get; set; }

    public double? SYARAT { get; set; }

    public string? NamaKirim { get; set; }

    public string? AlmKirim { get; set; }

    public string? NOSERI { get; set; }

    public string? PHD { get; set; }

    public string? Case1 { get; set; }

    public string? Case2 { get; set; }

    public string? Case3 { get; set; }

    public string? Case4 { get; set; }

    public string? Case5 { get; set; }

    public string? Shiping { get; set; }

    public string? MAWB { get; set; }

    public string? HAWB { get; set; }

    public string? NAMASIGN1 { get; set; }

    public string? NAMASIGN2 { get; set; }

    public string? NAMASIGN3 { get; set; }

    public string? JABATANSIGN1 { get; set; }

    public string? JABATANSIGN2 { get; set; }

    public string? JABATANSIGN3 { get; set; }

    public string? REV { get; set; }

    public string? DIVISION { get; set; }

    public string? NO_PEB { get; set; }

    public DateTime? TGL_PEB { get; set; }

    public string? Vessel3 { get; set; }

    public string? Vessel4 { get; set; }

    public string? REALISASI { get; set; }

    public DateTime? TGL_REALISASI { get; set; }

    public string? TYPE { get; set; }

    public string? Keterangan { get; set; }

    public string? UserID { get; set; }

    public string? HAPUS { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? StatusGL { get; set; }

    public string? Kode_CustomerGanti { get; set; }

    public bool? Hadiah { get; set; }

    public string? DOKU_KONTRAK { get; set; }

    public string? TipePRoject { get; set; }

    public double? GROSS { get; set; }

    public double? grandTotal { get; set; }

    public string? DOKU_PD { get; set; }

    public double? JmlKirim { get; set; }

    public double? PPhJasa { get; set; }

    public double? DPP { get; set; }

    public double? pphjasaTunai { get; set; }

    public string? Memo { get; set; }

    public string? Terbilang { get; set; }

    public string? TerbilangEnglish { get; set; }

    public string? Doku_paket { get; set; }

    public DateTime? tGLKirim { get; set; }

    public DateTime? tGLNPO { get; set; }

    public DateTime? tGLVerify { get; set; }

    public DateTime? tGL_PD { get; set; }

    public DateTime? tGL_Kontrak { get; set; }

    public string? nAMApROYEK { get; set; }

    public double? Pelunasan { get; set; }

    public string? Kode_PIC { get; set; }

    public string? DOKU_PROYEK { get; set; }

    public double? PPNNET { get; set; }

    public string? NewEPK { get; set; }

    public string? SalesLama { get; set; }

    public string? KodePajak { get; set; }

    public long PKBAS { get; set; }

    public DateTime? TglSewa1 { get; set; }

    public DateTime? TglSewa2 { get; set; }

    public int? PeriodSewa { get; set; }

    public double? Nilai_DPAR { get; set; }

    public string? Doku_FP { get; set; }

    public string? ProyekKe { get; set; }

    public double? DiskonOth { get; set; }

    public string? UserEdit { get; set; }

    public string? Kode_IDN { get; set; }

    public double? Rebet { get; set; }

    public string? Doku_Gabungan { get; set; }

    public DateTime? Tgl_Gabungan { get; set; }

    public string? EclipseID { get; set; }

    public string? ReportBenq { get; set; }

    public string? ModulSource { get; set; }

    public string? NPWPSub { get; set; }

    public double? CDOut { get; set; }

    public double? CDOutTunai { get; set; }

    public int? CekPreviewInvoice { get; set; }

    public DateTime? TglPreview { get; set; }

    public double Retensi { get; set; }

    public int Retensip { get; set; }

    public DateTime? CDOutExpired { get; set; }

    public byte[]? imgTTD { get; set; }

    public string? NamaTTD { get; set; }

    public bool? Validasi { get; set; }

    public DateTime? ValidasiTime { get; set; }

    public int? emailsent { get; set; }

    public DateTime? emaillastsent { get; set; }

    public string? NoMaterai { get; set; }

    public string? Kode_Meterai { get; set; }

    public double? Nilai_Meterai { get; set; }

    public string? JenisPajak { get; set; }

    public string? DokuBC40 { get; set; }

    public string? Order_Class { get; set; }

    public string? Order_Type { get; set; }

    public string? Kode_Area { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
