using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class FakturPajak
{
    public double? TGrossAmount { get; set; }

    public double? TDiskon { get; set; }

    public double? TNetAmount { get; set; }

    public double? TVAT { get; set; }

    public double? TVATRoundUp { get; set; }

    public double? TGrandTotal { get; set; }

    public double? TGrossAmountRP { get; set; }

    public double? TDiskonRP { get; set; }

    public double? TNetAmountRP { get; set; }

    public double? TVATRP { get; set; }

    public double? TVATRoundUpRP { get; set; }

    public double? TGrandTotalRP { get; set; }

    public string? Terbilang { get; set; }

    public string? TerbilangEnglish { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Doku_faktur { get; set; }

    public DateTime? Tgl_faktur { get; set; }

    public string? Kode_CustomerGabung { get; set; }

    public string? Nama_CustomerGabung { get; set; }

    public string? NPWP_Gabung { get; set; }

    public string? PKP_Gabung { get; set; }

    public string? Alamat_Gabung { get; set; }

    public string? Memo_Gabung { get; set; }

    public string? KodeRnd { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? TipeFaktur { get; set; }

    public string? kode_valas { get; set; }

    public double? kurs { get; set; }

    public double? TDP { get; set; }

    public string? MARKING { get; set; }

    public string? ODec { get; set; }

    public string? OMrk { get; set; }

    public string? OGrp { get; set; }

    public string? OPrc { get; set; }

    public string? Memo { get; set; }

    public string? Kode_Customer { get; set; }

    public string? kode_CustomerGANTi { get; set; }

    public string? TTD { get; set; }

    public long PKBAS { get; set; }

    public string? EFaktur { get; set; }

    public string? Kode_IDN { get; set; }

    public string? Proyekkd { get; set; }

    public string? csvI { get; set; }

    public string? DokuBC40 { get; set; }
}
