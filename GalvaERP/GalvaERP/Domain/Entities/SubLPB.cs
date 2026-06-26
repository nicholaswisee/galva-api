using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SubLPB
{
    public long id_sub_lpb { get; set; }

    public string? kode_BRGganti { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Doku_PO { get; set; }

    public string? Doku_SPPB { get; set; }

    public short? NoUrutSPPB { get; set; }

    public string? Kode_Brg { get; set; }

    public string? Kode_Gudang { get; set; }

    public double? Jumlah { get; set; }

    public double? JML_LPB_Temp { get; set; }

    public double? JML_Retur { get; set; }

    public double? JML_Retur_Temp { get; set; }

    public double? JumlahKeluar { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Harga { get; set; }

    public double? Nilai { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? PPN { get; set; }

    public double? PPnBm { get; set; }

    public double? NilaiDistribusi { get; set; }

    public double? JML_BYR { get; set; }

    public short? TERM_BYR { get; set; }

    public DateTime? TglCreate { get; set; }

    public DateTime? TGL_BAYAR { get; set; }

    public string? MEMO1 { get; set; }

    public string? STP { get; set; }

    public short? JMRLPB { get; set; }

    public string? NAMAUSER { get; set; }

    public double? Kurs { get; set; }

    public string? Kode_Dept_PO { get; set; }

    public string? Ext_Doku_PO { get; set; }

    public string? Keterangan { get; set; }

    public string? SuratJalan { get; set; }

    public DateTime? Tgl_PO { get; set; }

    public string? TempNama { get; set; }

    public double? TempOrder { get; set; }

    public string? Estimated { get; set; }

    public double? Urut { get; set; }

    public string? KodeRnd { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? Model { get; set; }
}
