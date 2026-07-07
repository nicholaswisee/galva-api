using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class VoucherAP
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? TglDoku { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Doku_LPB { get; set; }

    public string? Doku_PO { get; set; }

    public string? TipeBiaya { get; set; }

    public DateTime? TglDokuLPB { get; set; }

    public DateTime? TglDokuPO { get; set; }

    public short? Syarat { get; set; }

    public DateTime? TglJatuhTempo { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public double? KursPajak { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? PPn { get; set; }

    public double? PPnBm { get; set; }

    public double? Misc { get; set; }

    public double? NilaiLPB { get; set; }

    public double? Nilai { get; set; }

    public string? Keterangan { get; set; }

    public string? STS { get; set; }

    public string? Tipe { get; set; }

    public short? NoUrut { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? UserID { get; set; }

    public string? Doku_FP { get; set; }

    public DateTime? Tgl_FP { get; set; }

    public string? EFaktur { get; set; }

    public double? PPnTunai { get; set; }

    public string? Kode_IDN { get; set; }

    public string? ModulSource { get; set; }

    public string? MajorDiskon { get; set; }

    public double? DPPNilaiLain { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}