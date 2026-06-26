using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class TTPRetur
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? TGL { get; set; }

    public DateTime? Tgl_Ganti { get; set; }

    public string? Doku_TTP { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Kode_SubCustomer { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Kode_Gudang { get; set; }

    public double? Total { get; set; }

    public string? Kode_Sales { get; set; }

    public string? Destination { get; set; }

    public string? NamaKirim { get; set; }

    public string? AlmKirim { get; set; }

    public string? Sts { get; set; }

    public string? Sts_Temp { get; set; }

    public string? UserID { get; set; }

    public string? UserCreate { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? Kode_CustomerGanti { get; set; }

    public bool? Validasi { get; set; }

    public string? usernd { get; set; }

    public string? DOKU_SJ { get; set; }
}
