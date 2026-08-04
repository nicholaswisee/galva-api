using System;

namespace GalvaERP.Features.PurchaseRequisitions.DTOs;

public class PRListDto
{
    public string Doku { get; set; } = string.Empty;
    public DateTime Tgl { get; set; }
    public string? Kode_Dept { get; set; }
    public string? Status { get; set; }
    public bool? StsVerify { get; set; }
    public string? ETag { get; set; }
}
