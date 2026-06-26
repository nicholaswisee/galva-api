using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class subTTP
{
    public long PKbas { get; set; }

    public string? kode_BRGganti { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Doku_TTP { get; set; }

    public string? Kode_Customer { get; set; }

    public string? Kode_Brg { get; set; }

    public string? Kode_brgLama { get; set; }

    public string? NAma_brg { get; set; }

    public string? Unit { get; set; }

    public string? Kode_Gudang { get; set; }

    public string? Kode_GudangPinjaman { get; set; }

    public string? Kode_gudangLama { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Kode_deptLama { get; set; }

    public string? Alias { get; set; }

    public string? JumlahAwal { get; set; }

    public double? Jumlah { get; set; }

    public double? JumlahTemp { get; set; }

    public double? JumlahRetur { get; set; }

    public double? JumlahReturTemp { get; set; }

    public double? JumlahReturSem { get; set; }

    public double? JumlahReturSakti { get; set; }

    public double? JumlahSisa { get; set; }

    public double? Harga { get; set; }

    public double? HPP { get; set; }

    public double? Total { get; set; }

    public string? Ket { get; set; }

    public short? NoUrut { get; set; }

    public string? KodeRnd { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? KODE_CUSTOMERGANTI { get; set; }
}
