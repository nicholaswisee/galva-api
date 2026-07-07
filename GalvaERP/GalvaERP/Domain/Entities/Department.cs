using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

// ponytail: class kept as "Department" (not renamed to "Dept") to avoid
//           DbSet rename cascade; EF ToTable("Dept") handles the table name.
public partial class Department
{
    public long id_dept { get; set; }

    public string? KodeGTC { get; set; }

    public string? KodeEPK { get; set; }

    public string? Kode { get; set; }

    public string? Nama { get; set; }

    public string? NamaUser { get; set; }

    public DateTime? TglUpDate { get; set; }

    public string? Head { get; set; }

    public string? Chief { get; set; }

    public string? Staff { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? NewEPK { get; set; }

    public int? SYARAT { get; set; }

    public bool? HideReport { get; set; }

    public string? dept_group { get; set; }

    public bool? NonAktif { get; set; }

    public DateTime? NonAktifTime { get; set; }

    public string? Kode_Master_Department { get; set; }
}