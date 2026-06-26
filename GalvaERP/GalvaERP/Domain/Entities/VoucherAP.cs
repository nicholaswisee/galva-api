using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class VoucherAP
{
    public long PKbas { get; set; }

    public string? Doku { get; set; }

    public DateTime? Tgl { get; set; }

    public string? Kode_Supplier { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Kode_Bank { get; set; }

    public string? Kode_Valas { get; set; }

    public double? Kurs { get; set; }

    public double? Nilai { get; set; }

    public double? PPn { get; set; }

    public double? PPnBm { get; set; }

    public double? Diskon { get; set; }

    public double? DiskonTunai { get; set; }

    public double? Misc { get; set; }

    public string? STS { get; set; }

    public string? Status { get; set; }

    public string? Keterangan { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? StatusGL { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
