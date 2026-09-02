using System.Text.Json.Serialization;

namespace GalvaERP.Features.PurchaseOrders.DTOs;

public class ConfirmedPurchaseOrderListDto
{
    public string Doku { get; set; } = string.Empty;
    public DateTime? Tgl { get; set; }
    public string? Doku_PO { get; set; }
    public string? Doku_POSem { get; set; }
    public string? Kode_Supplier { get; set; }
    public string? SupplierName { get; set; }
    public double? Nilai { get; set; }
    public string? STS { get; set; }
    public string? ETag { get; set; }
}

public record ConfirmedPurchaseOrderDetailDto(
    string Doku,
    DateTime? Tgl,
    string? Doku_PO,
    string? Doku_POSem,
    string? Kode_Supplier,
    string? SupplierName,
    string? Kode_dept,
    string? Kode_Valas,
    double? Kurs,
    string? ContactPr,
    DateTime? Psd,
    DateTime? Etd,
    string? Memo,
    double? Nilai,
    double? PPN,
    double? Diskon,
    string? STS,
    string ETag,
    List<ConfirmedPurchaseOrderLineDto> Lines);

public record ConfirmedPurchaseOrderLineDto(
    long id_sub_po,
    [property: JsonPropertyName("id_sub_po_confirmation")] long LegacySubPOId,
    string? Kode_Brg,
    double? Jumlah,
    double? Harga,
    double? Total,
    string? Kode_Gudang,
    string? Note);
