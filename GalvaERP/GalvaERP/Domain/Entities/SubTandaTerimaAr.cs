using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SubTandaTerimaAr
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Doku_Faktur { get; set; }

    public string? Doku_LPB { get; set; }

    public string? SuratJalan { get; set; }

    public string? Giro { get; set; }

    public DateTime? TglGiro { get; set; }

    public double? Nilai { get; set; }

    public double? DiskonTunai { get; set; }

    public double? TotalNilai { get; set; }

    public string? STS { get; set; }

    public string? Doku_Muka { get; set; }

    public short? NoUrut { get; set; }

    public string? Cara { get; set; }

    public string? Kode_Valas { get; set; }

    public string? Kode_ValasBayar { get; set; }

    public double? NilaiLocal { get; set; }

    public double? NilaiForeign { get; set; }

    public double? Kurs { get; set; }

    public double? KursBayar { get; set; }

    public double? KursLocal { get; set; }

    public double? KursKonversi { get; set; }

    public string? Kode_Bank { get; set; }

    public double? SelisihTagih { get; set; }

    public string? Keterangan { get; set; }

    public string? Status { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string MajorRef { get; set; } = null!;

    public string? Reference { get; set; }

    public string? DokuKwitansiAR { get; set; }
}
