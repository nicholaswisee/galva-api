using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SubBayar
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Doku_LPB { get; set; }

    public string? Doku_PO { get; set; }

    public string? Doku_Voucher { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_Brg { get; set; }

    public double? Nilai { get; set; }

    public double? NilaiBayar { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public short? NoUrut { get; set; }

    public string? Keterangan { get; set; }

    public string? UserID { get; set; }

    public DateTime? EntryDate { get; set; }
}
