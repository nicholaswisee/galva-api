using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class SaldoAP
{
    public long PKbas { get; set; }

    public string? Kode_Supplier { get; set; }

    public double? Awal { get; set; }
}
