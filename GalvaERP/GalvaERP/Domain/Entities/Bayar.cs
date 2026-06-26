using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Bayar
{
    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_BankSupplier { get; set; }

    public string? Keterangan { get; set; }

    public double? NilaiKas { get; set; }

    public double? NilaiGiro { get; set; }

    public double? NilaiAJE { get; set; }

    public double? NilMuka { get; set; }

    public string? STS { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public double? Selisih_Bayar { get; set; }

    public string? Cara { get; set; }

    public string? Jenis { get; set; }

    public string? Hapus { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? StatusGL { get; set; }

    public long PKindex { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
