using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Barang
{
    public string? Kode { get; set; }

    public string? Nama { get; set; }

    public string? Merk { get; set; }

    public string? Satuan { get; set; }

    public double? Harga { get; set; }
}
