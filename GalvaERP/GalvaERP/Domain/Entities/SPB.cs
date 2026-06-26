using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SPB
{
    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public DateTime? TglVerify { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Kode_SubCustomer { get; set; }

    public string? NPO { get; set; }

    public DateTime? TglNPO { get; set; }

    public string? Hubungi { get; set; }

    public double? JmlKirim { get; set; }

    public DateTime? TglKirim { get; set; }

    public string? NamaKirim { get; set; }

    public string? AlmKirim { get; set; }

    public double? PPn { get; set; }

    public double? PPnTunai { get; set; }

    public double? PPnBm { get; set; }

    public double? PPnBmTunai { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? Total { get; set; }

    public double? Nilai { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public string? Sts { get; set; }

    public string? Status { get; set; }

    public DateTime? Waktu { get; set; }

    public string? Ship { get; set; }

    public string? Pay { get; set; }

    public string? Lihat { get; set; }

    public string? Kode_Sales { get; set; }

    public double? JMPRN { get; set; }

    public double? Syarat { get; set; }

    public string? Kode_Dept { get; set; }

    public bool? StsVerify { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? Kode_CustomerGanti { get; set; }

    public string? NamaProyek { get; set; }

    public string? Doku_PD { get; set; }

    public DateTime? tgl_DokuPD { get; set; }

    public string? Doku_Kontrak { get; set; }

    public DateTime? tgl_Kontrak { get; set; }

    public string? TipePRoject { get; set; }

    public string? Terbilang { get; set; }

    public string? TerbilangEnglish { get; set; }

    public double? kurspajak { get; set; }

    public DateTime? tgl_PD { get; set; }

    public double? PPHJasa { get; set; }

    public double? GROSS { get; set; }

    public double? GRANDTOTAL { get; set; }

    public double? DPP { get; set; }

    public double? PPHJasaTunai { get; set; }

    public string? MEMO { get; set; }

    public bool? Hadiah { get; set; }

    public string? NAMA_PD { get; set; }

    public double? DPP_PD { get; set; }

    public string? Kode_PIC { get; set; }

    public bool? ProInv { get; set; }

    public string? NewEPK { get; set; }

    public string? SalesLama { get; set; }

    public long id_spb { get; set; }

    public double? HangusSO { get; set; }

    public string? NoteHangusSO { get; set; }

    public DateTime? TglSewa1 { get; set; }

    public DateTime? TglSewa2 { get; set; }

    public int? PeriodSewa { get; set; }

    public string? DokuSFA { get; set; }

    public DateTime? TglDokuSFA { get; set; }

    public string? Titip { get; set; }

    public double? DiskonOth { get; set; }

    public double? CDLangsung { get; set; }

    public string Jenis { get; set; } = null!;

    public string KirimKd { get; set; } = null!;

    public string? Kode_IDN { get; set; }

    public string? Doku_Sewa { get; set; }

    public double? Rebet { get; set; }

    public string? lokasi { get; set; }

    public string? EclipseID { get; set; }

    public string? MCCode { get; set; }

    public string? Kode_MarketSegment { get; set; }

    public string? NIK { get; set; }

    public string? ModulSource { get; set; }

    public string? ClaimCode { get; set; }

    public double? CDLangsungPersen { get; set; }

    public string? Kode_MarketSegmentGrup { get; set; }

    public string? Doku_LPB { get; set; }

    public string? NPWPSub { get; set; }

    public double? KursJual { get; set; }

    public double? CDOut { get; set; }

    public double? CDOutPersen { get; set; }

    public double? CDOutTunai { get; set; }

    public int? CDOutHari { get; set; }

    public DateTime? CDOutTglAwal { get; set; }

    public DateTime? CDOutTglAkhir { get; set; }

    public string? CDOutBasicCal { get; set; }

    public string? CDOutDayCal { get; set; }

    public string? NamaPenerima { get; set; }

    public string Kode_MarketSegmentGrupOld { get; set; } = null!;

    public string? BusinessModel { get; set; }

    public string? Order_Class { get; set; }

    public string? Order_Type { get; set; }

    public string? Kode_Area { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
