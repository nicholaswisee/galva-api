using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Gudang
{
    public string? Kode { get; set; }

    public string? KodeLama { get; set; }

    public string? Nama { get; set; }

    public bool? Aktif { get; set; }

    public string? NoCounter1 { get; set; }

    public string? NoCounter2 { get; set; }

    public string? NoCounter3 { get; set; }

    public bool? TipeIC { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? Kode_Area { get; set; }

    public short? KodeNum { get; set; }

    public string? Alamat1 { get; set; }

    public string? Alamat2 { get; set; }

    public string? Kota { get; set; }

    public bool? FreeSN { get; set; }

    public bool? TipeSRV { get; set; }

    public bool? TipeTTP { get; set; }

    public string? NewEPK { get; set; }

    public long id_gudang { get; set; }

    public string? Email { get; set; }

    public string? PIC { get; set; }

    public bool? ByPasSN { get; set; }

    public string? KodeLokasi { get; set; }

    public string? KodeOpname { get; set; }

    public string? Kode_GudangOpname { get; set; }

    public bool? SyncToCMG { get; set; }

    public string? GudangSync { get; set; }

    public bool? Ecom { get; set; }

    public bool? HideReport { get; set; }

    public bool? WHBlocked { get; set; }

    public string? Kode_AreaOld { get; set; }
}
