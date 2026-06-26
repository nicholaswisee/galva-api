using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Bank
{
    public string? Kode { get; set; }

    public string? KodeLama { get; set; }

    public string? Nama { get; set; }

    public bool? LookupBank { get; set; }

    public string? Major { get; set; }

    public string? Kode_JenisBayar { get; set; }

    public string? Reference { get; set; }

    public string? Kode_Valas { get; set; }

    public string? Alamat1 { get; set; }

    public string? Alamat2 { get; set; }

    public string? Kota { get; set; }

    public string? KodePos { get; set; }

    public string? Telepon { get; set; }

    public string? Fax { get; set; }

    public string? AC { get; set; }

    public string? AN { get; set; }

    public double? Awal { get; set; }

    public double? Masuk { get; set; }

    public double? Keluar { get; set; }

    public string? Kode_Area { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public long PKindex { get; set; }

    public string? MajorPajak { get; set; }

    public int? BpPPn { get; set; }

    public string? PPh23List { get; set; }

    public int? Diskontinu { get; set; }

    public DateTime? TglDiskontinu { get; set; }
}
