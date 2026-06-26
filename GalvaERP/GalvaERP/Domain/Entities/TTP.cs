using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class TTP
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? TGL { get; set; }

    public string? kode_area { get; set; }

    public string? Kode_Customer { get; set; }

    public string? kode_customerLama { get; set; }

    public string? Nama_customer { get; set; }

    public string? Kode_SubCustomer { get; set; }

    public string? Kode_Dept { get; set; }

    public string? kode_deptlama { get; set; }

    public string? Kode_Gudang { get; set; }

    public string? Kode_gudanglama { get; set; }

    public string? Kode_Sales { get; set; }

    public string? Nama_sales { get; set; }

    public string? Destination { get; set; }

    public string? NamaKirim { get; set; }

    public string? AlmKirim { get; set; }

    public string? Sts { get; set; }

    public string? Sts_Temp { get; set; }

    public string? UserID { get; set; }

    public string? UserUpdate { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public DateTime? EntryUpdate { get; set; }

    public double? Jumlah { get; set; }

    public double? JumlahRetur { get; set; }

    public double? JumlahSisa { get; set; }

    public string? Kode_CustomerGanti { get; set; }

    public bool? Validasi { get; set; }

    public string? DOKU_SJ { get; set; }
}
