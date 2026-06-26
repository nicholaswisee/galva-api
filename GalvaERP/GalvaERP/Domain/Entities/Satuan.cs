using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Satuan
{
    public long id_satuan { get; set; }

    public string? Kode { get; set; }

    public string? Nama { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }
}
