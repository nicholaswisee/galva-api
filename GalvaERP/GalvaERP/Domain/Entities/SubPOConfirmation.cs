using System;

namespace GalvaERP.Domain.Entities;

public partial class SubPOConfirmation
{
    public string? Doku { get; set; }

    public long? id_sub_po { get; set; }

    public string? Kode_Brg { get; set; }

    public double? Jumlah { get; set; }

    public double? Harga { get; set; }

    public double? Total { get; set; }

    public string? Kode_Gudang { get; set; }

    public string? Note { get; set; }

    public DateTime? EntryDate { get; set; }

    public long id_sub_po_confirmation { get; set; }
}
