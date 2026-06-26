using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SubSPB
{
    public string? kode_brgGanti { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Brg { get; set; }

    public string? Kode_Tujuan { get; set; }

    public string? Kode_Gudang { get; set; }

    public string? Alias { get; set; }

    public string? Spec { get; set; }

    public double? Harga { get; set; }

    public double? Jumlah { get; set; }

    public double? JumlahTemp { get; set; }

    public double? Nilai { get; set; }

    public double? Realisasi { get; set; }

    public double? JmlKirim { get; set; }

    public double? JmlKirimTemp { get; set; }

    public double? JmlKirimSem { get; set; }

    public double? JumlahVerify { get; set; }

    public double? JumlahVerifyTemp { get; set; }

    public double? SisaOrder { get; set; }

    public DateTime? TglKirim { get; set; }

    public string? AlmKirim { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? PPn { get; set; }

    public string? PPnBm { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public short? NoUrut { get; set; }

    public string? Kode_Sales { get; set; }

    public string? Sts { get; set; }

    public string? Kode_Dept { get; set; }

    public string? KodeRnd { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? Hapus { get; set; }

    public double? kursPajak { get; set; }

    public string? Doku_Paket { get; set; }

    public string? kode_Paket { get; set; }

    public string? Nama_Paket { get; set; }

    public DateTime? tgl_Paket { get; set; }

    public double? PPhJasa { get; set; }

    public double? Gross { get; set; }

    public double? Dpp { get; set; }

    public double? SubTotal { get; set; }

    public string? MajorPSD { get; set; }

    public string? MajorAR { get; set; }

    public string? MajorHPP { get; set; }

    public string? MajorCustomer { get; set; }

    public string? ReferenceCustomer { get; set; }

    public string? MajorPPn { get; set; }

    public string? MajorDiskon { get; set; }

    public string? MajorPPnBM { get; set; }

    public string? MajorPPhJasa { get; set; }

    public double? HargaNet { get; set; }

    public double? HargaPPnNet { get; set; }

    public double? PPnNet { get; set; }

    public double? DiskonNet { get; set; }

    public int? SN { get; set; }

    public string? TIPEPROJECT { get; set; }

    public double? HPPGLOBAL { get; set; }

    public double? HargaPaket { get; set; }

    public string? Memo { get; set; }

    public string? NewEPK { get; set; }

    public string? SalesLama { get; set; }

    public long id_sub_spb { get; set; }

    public double? JumlahMin { get; set; }

    public string? DokuSFA { get; set; }

    public string Jenis { get; set; } = null!;

    public string KirimKd { get; set; } = null!;

    public string Status { get; set; } = null!;

    public double? Jumhar { get; set; }

    public string? Proyekkd { get; set; }

    public string? CustKd { get; set; }

    public string? SerialNumber { get; set; }

    public string? Nm_Brg { get; set; }

    public string? InfoCM { get; set; }

    public string? AliasCode { get; set; }
}
