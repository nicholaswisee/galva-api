using System;

namespace GalvaERP.Domain.Entities;

public partial class POConfirmation
{
    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Doku_PO { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_dept { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public string? ContactPr { get; set; }

    public DateTime? Psd { get; set; }

    public DateTime? Etd { get; set; }

    public string? Memo { get; set; }

    public double? Nilai { get; set; }

    public double? PPN { get; set; }

    public double? Diskon { get; set; }

    public string? STS { get; set; }

    public DateTime? EntryDate { get; set; }

    public long id_po_confirmation { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
