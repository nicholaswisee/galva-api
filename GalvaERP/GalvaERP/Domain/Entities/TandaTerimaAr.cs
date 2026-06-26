using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class TandaTerimaAr
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Kode_BankCustomer { get; set; }

    public string? Keterangan { get; set; }

    public double NilKas { get; set; }

    public double NilGiro { get; set; }

    public double NilAJE { get; set; }

    public double NilMuka { get; set; }

    public string? STS { get; set; }

    public string? Kode_Valas { get; set; }

    public double Kurs { get; set; }

    public double Selisih_Bayar { get; set; }

    public string? Cara { get; set; }

    public string? Jenis { get; set; }

    public string? Hapus { get; set; }

    public string? UserID { get; set; }

    public string? EntryDate { get; set; }

    public string? StatusGL { get; set; }

    public string? StsTipe { get; set; }

    public double Selisih_Tagih { get; set; }

    public double Nilai { get; set; }

    public string? InUse { get; set; }

    public string? UserArea { get; set; }
}
