using System;
using System.Collections.Generic;

namespace GalvaERP.Domain.Entities;

public partial class Supplier
{
    public long PKbas { get; set; }

    public string? KodeEPK { get; set; }

    public string? KodeGTC { get; set; }

    public DateTime? TglMasuk { get; set; }

    public string? Kode { get; set; }

    public string? Nama { get; set; }

    public string? KodeLama { get; set; }

    public string? Kode_Dept { get; set; }

    public string? Kode_Area { get; set; }

    public string? NPWP { get; set; }

    public string? PKP { get; set; }

    public bool? BankLC { get; set; }

    public string? Contact1 { get; set; }

    public string? Contact2 { get; set; }

    public string? Contact3 { get; set; }

    public string? Contact4 { get; set; }

    public string? Kode_Usaha { get; set; }

    public string? Kode_Sales { get; set; }

    public string? MOS { get; set; }

    public short? Syarat { get; set; }

    public double? Limit { get; set; }

    public double? Diskon { get; set; }

    public string? PHD { get; set; }

    public double? PPN { get; set; }

    public string? Major { get; set; }

    public string? Reference { get; set; }

    public string? Alamat1 { get; set; }

    public string? Alamat2 { get; set; }

    public string? Kota { get; set; }

    public string? Negara { get; set; }

    public string? KodePos { get; set; }

    public string? Telepon { get; set; }

    public string? Fax { get; set; }

    public string? Benua { get; set; }

    public string? Alamat1Pabrik { get; set; }

    public string? Alamat2Pabrik { get; set; }

    public string? KotaPabrik { get; set; }

    public string? NegaraPabrik { get; set; }

    public string? KodePosPabrik { get; set; }

    public string? TeleponPabrik { get; set; }

    public string? FaxPabrik { get; set; }

    public string? BenuaPabrik { get; set; }

    public string? Status { get; set; }

    public string? TipeHarga { get; set; }

    public double? Muka { get; set; }

    public double? Giro { get; set; }

    public double? Awal { get; set; }

    public double? D1 { get; set; }

    public double? D2 { get; set; }

    public double? D3 { get; set; }

    public double? D4 { get; set; }

    public double? D5 { get; set; }

    public double? D6 { get; set; }

    public double? D7 { get; set; }

    public double? D8 { get; set; }

    public double? D9 { get; set; }

    public double? D10 { get; set; }

    public double? D11 { get; set; }

    public double? D12 { get; set; }

    public double? K1 { get; set; }

    public double? K2 { get; set; }

    public double? K3 { get; set; }

    public double? K4 { get; set; }

    public double? K5 { get; set; }

    public double? K6 { get; set; }

    public double? K7 { get; set; }

    public double? K8 { get; set; }

    public double? K9 { get; set; }

    public double? K10 { get; set; }

    public double? K11 { get; set; }

    public double? K12 { get; set; }

    public double? R1 { get; set; }

    public double? R2 { get; set; }

    public double? R3 { get; set; }

    public double? R4 { get; set; }

    public double? R5 { get; set; }

    public double? R6 { get; set; }

    public double? R7 { get; set; }

    public double? R8 { get; set; }

    public double? R9 { get; set; }

    public double? R10 { get; set; }

    public double? R11 { get; set; }

    public double? R12 { get; set; }

    public string? MTU { get; set; }

    public double? VMuka { get; set; }

    public double? VGiro { get; set; }

    public double? VAwal { get; set; }

    public double? VD1 { get; set; }

    public double? VD2 { get; set; }

    public double? VD3 { get; set; }

    public double? VD4 { get; set; }

    public double? VD5 { get; set; }

    public double? VD6 { get; set; }

    public double? VD7 { get; set; }

    public double? VD8 { get; set; }

    public double? VD9 { get; set; }

    public double? VD10 { get; set; }

    public double? VD11 { get; set; }

    public double? VD12 { get; set; }

    public double? VK1 { get; set; }

    public double? VK2 { get; set; }

    public double? VK3 { get; set; }

    public double? VK4 { get; set; }

    public double? VK5 { get; set; }

    public double? VK6 { get; set; }

    public double? VK7 { get; set; }

    public double? VK8 { get; set; }

    public double? VK9 { get; set; }

    public double? VK10 { get; set; }

    public double? VK11 { get; set; }

    public double? VK12 { get; set; }

    public double? VR1 { get; set; }

    public double? VR2 { get; set; }

    public double? VR3 { get; set; }

    public double? VR4 { get; set; }

    public double? VR5 { get; set; }

    public double? VR6 { get; set; }

    public double? VR7 { get; set; }

    public double? VR8 { get; set; }

    public double? VR9 { get; set; }

    public double? VR10 { get; set; }

    public double? VR11 { get; set; }

    public double? VR12 { get; set; }

    public string? TipeHutang { get; set; }

    public string? UserID { get; set; }

    public string? Hapus { get; set; }

    public DateTime? EntryDate { get; set; }

    public string? ServerFrom { get; set; }

    public string? Alamat1Pajak { get; set; }

    public string? Alamat2Pajak { get; set; }

    public string? Jenis { get; set; }

    public string? Keterangan { get; set; }

    public string? KodePosPajak { get; set; }

    public string? KodeTrim { get; set; }

    public string? KotaPajak { get; set; }

    public string? LOGID { get; set; }

    public string? NamaPajak { get; set; }

    public string? NamaTrim { get; set; }

    public string? NegaraPajak { get; set; }

    public double? PPNGST { get; set; }

    public string? Propinsi { get; set; }

    public string? TipeDiskon { get; set; }

    public string? TransferTime { get; set; }

    public string? SupGroup { get; set; }

    public string? Kode_buyer { get; set; }

    public string? SupGroupName { get; set; }

    public bool? bPPN { get; set; }

    public bool? VoucherSistem { get; set; }

    public bool? Aktif { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public string? Kode_Customer { get; set; }

    public byte[] RowVersion { get; set; } = null!;
}
