using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SUBFAKTUR
{
    public string? kode_BRGganti { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Kode_Gudang { get; set; }

    public string? Doku_SJ { get; set; }

    public string? Doku_SPB { get; set; }

    public string? NPO { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Kode_Brg { get; set; }

    public string? Alias { get; set; }

    public string? Spec { get; set; }

    public double? Jumlah { get; set; }

    public double? JumlahTemp { get; set; }

    public double? JumlahRetur { get; set; }

    public double? JumlahReturTemp { get; set; }

    public double? Harga { get; set; }

    public double? Hpp { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? PPN { get; set; }

    public double? PPnBm { get; set; }

    public double? Nilai { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public byte? Comercial { get; set; }

    public short? NoUrut { get; set; }

    public string? Hapus { get; set; }

    public string? KodeRnd { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }

    public double? JML_Retur_Temp { get; set; }

    public string? Kode_CustomerGanti { get; set; }

    public double? HARGAPAKET { get; set; }

    public string? TipePRoject { get; set; }

    public double? JmlKirim { get; set; }

    public double? JmlKirimTemp { get; set; }

    public double? JumlahVerify { get; set; }

    public double? SisaOrder { get; set; }

    public double? PPnNet { get; set; }

    public double? HargaNet { get; set; }

    public double? HargaPPnNet { get; set; }

    public double? DiskonNet { get; set; }

    public double? Realisasi { get; set; }

    public double? PPhJasa { get; set; }

    public double? KursPajak { get; set; }

    public double? SubTotal { get; set; }

    public string? MajorPSD { get; set; }

    public string? MajorAR { get; set; }

    public string? MajorHPP { get; set; }

    public string? MajorCustomer { get; set; }

    public string? referencecustomer { get; set; }

    public string? MajorPPn { get; set; }

    public string? MajordISKON { get; set; }

    public string? Kode_tujuan { get; set; }

    public string? Doku_paket { get; set; }

    public string? Kode_paket { get; set; }

    public string? Nama_paket { get; set; }

    public string? AlmKirim { get; set; }

    public DateTime? TglKirim { get; set; }

    public DateTime? tgl_paket { get; set; }

    public string? sts { get; set; }

    public string? Kode_Sales { get; set; }

    public double? HPPGLOBAL { get; set; }

    public string? MODEL { get; set; }

    public string? MAJORPPHJASA { get; set; }

    public string? MAJORPPNBM { get; set; }

    public string? MAJORRETUR { get; set; }

    public string? Memo { get; set; }

    public double? gross { get; set; }

    public DateTime? tgl_kirim { get; set; }

    public string? MajorPPbBm { get; set; }

    public string? MajorPPbnBm { get; set; }

    public string? NewEPK { get; set; }

    public string? SalesLama { get; set; }

    public long PKindex { get; set; }

    public double? JumlahMin { get; set; }

    public string ProyekKe { get; set; } = null!;

    public string? Proyekkd { get; set; }

    public string? InfoCM { get; set; }

    public string? AliasCode { get; set; }
}
