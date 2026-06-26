using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Category
{
    public string? Kode { get; set; }

    public string? Nama { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public long id_category { get; set; }
}
