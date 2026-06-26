using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class PO
{
    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Doku_POSem { get; set; }

    public string? DokuVendor { get; set; }

    public DateTime? TglDokuVendor { get; set; }

    public string? BLAWB { get; set; }

    public string? Carrier { get; set; }

    public string? Vessel { get; set; }

    public string? Arrival { get; set; }

    public string? PIUD { get; set; }

    public DateTime? TglPIUD { get; set; }

    public string? Ship { get; set; }

    public DateTime? TglShip { get; set; }

    public DateTime? TglDeparture { get; set; }

    public string? Discharge { get; set; }

    public string? Loading { get; set; }

    public string? CountryOrigin { get; set; }

    public DateTime? TglCountryOrigin { get; set; }

    public double? Weight { get; set; }

    public string? Memo { get; set; }

    public string? ContactPr { get; set; }

    public short? Syarat { get; set; }

    public string? Revisi { get; set; }

    public string? Terms { get; set; }

    public double? PPH22 { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? PPN { get; set; }

    public double? PPnBM { get; set; }

    public double? Nilai { get; set; }

    public string? Kode_dept { get; set; }

    public string? LC { get; set; }

    public DateTime? Tgl_Pengiriman { get; set; }

    public DateTime? Tgl_Pembayaran { get; set; }

    public string? Pembayaran { get; set; }

    public string? Penyelesaian { get; set; }

    public string? ADDITIONAL { get; set; }

    public string? PEMBUATAN { get; set; }

    public string? Doku_SPPB { get; set; }

    public short? Jml_Print { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public DateTime? Wkt { get; set; }

    public string? DokuExt { get; set; }

    public string? STS { get; set; }

    public string? MOS { get; set; }

    public string? Packing { get; set; }

    public string? Sign { get; set; }

    public string? Tipe { get; set; }

    public string? STSPrint { get; set; }

    public bool? StsVerify { get; set; }

    public DateTime? TglVerify { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? Kode_buyer { get; set; }

    public double? BiayaMasuk { get; set; }

    public double? BiayaMasukP { get; set; }

    public long id_po { get; set; }

    public string? Kode_IDN { get; set; }

    public string? ModulSource { get; set; }

    public bool? CreatedInWMS { get; set; }

    public string? CreatedByInWMS { get; set; }

    public DateTime? CreatedDateInWMS { get; set; }

    public double? DPPNilaiLain { get; set; }

    public double? PPnTunai { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
