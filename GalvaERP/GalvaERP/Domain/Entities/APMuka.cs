using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class APMuka
{
    public string? Doku { get; set; }

    public DateTime? TglDoku { get; set; }

    public string? Doku_PO { get; set; }

    public string? Doku_Bayar { get; set; }

    public DateTime? TglDokuBayar { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_Bank { get; set; }

    public string? Giro { get; set; }

    public double? PPn { get; set; }

    public double? NilaiBruto { get; set; }

    public double? NilaiKas { get; set; }

    public double? NilaiKasTerpakai { get; set; }

    public DateTime? TglGiro { get; set; }

    public DateTime? TglCair { get; set; }

    public string? Sts { get; set; }

    public double? Kompensasi { get; set; }

    public DateTime? Kirim { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public string? Memo { get; set; }

    public string? Tipe { get; set; }

    public string? NoSeri { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }

    public double? NilaiGiro { get; set; }

    public double? NilaiGiroTerpakai { get; set; }

    public string? NamaUser { get; set; }

    public string? Jenis { get; set; }

    public long PKindex { get; set; }
}
