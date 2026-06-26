using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class AwalBank
{
    public double? Kode { get; set; }

    public string? AC { get; set; }

    public string? Nama { get; set; }

    public string? Kode_Valas { get; set; }

    public long PKindex { get; set; }
}
