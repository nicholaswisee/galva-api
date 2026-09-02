using System.Text.Json.Serialization;

namespace GalvaERP.Features.GoodsReceipts.DTOs;

public sealed class GRLineItemDto
{
    public string Kode_Brg { get; init; } = string.Empty;
    public double Jumlah { get; init; }
    public double Harga { get; init; }
    public string? Kode_Gudang { get; init; }
    public long id_sub_po { get; init; }

    [JsonPropertyName("id_sub_po_confirmation")]
    public long LegacySubPOId { get; init; }

    public long ResolvedSubPOId => id_sub_po > 0 ? id_sub_po : LegacySubPOId;
}
