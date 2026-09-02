#!/usr/bin/env bash
set -euo pipefail

root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)/GalvaERP"
context="$root/Infrastructure/Data/AppDbContext.cs"

fail() {
    printf 'Schema contract failed: %s\n' "$1" >&2
    exit 1
}

if grep -R -E --exclude-dir=bin --exclude-dir=obj \
    'POConfirmation|SubPOConfirmation|Doku_PCF|id_sub_po_confirmation' "$root/Domain/Entities" >/dev/null || \
    grep -E 'DbSet<(POConfirmation|SubPOConfirmation)>|Entity<(POConfirmation|SubPOConfirmation)>|ToTable\("(POConfirmation|SubPOConfirmation)"\)|Doku_PCF|id_sub_po_confirmation' "$context" >/dev/null; then
    fail 'removed PCF schema is still mapped'
fi

if grep -F 'ToTable("Bayar")' "$context" >/dev/null || grep -F 'ToTable("SubBayar")' "$context" >/dev/null; then
    fail 'removed payment tables are still mapped'
fi

for contract in 'DbSet<POSem>' 'DbSet<SubPOSem>' 'ToTable("POSem")' 'ToTable("SubPOSem")' 'ToTable("Hiapt06")' 'ToTable("Hiapt02")'; do
    grep -F "$contract" "$context" >/dev/null || fail "missing $contract"
done

python3 - "$context" "$root" <<'PY'
import pathlib
import sys

context = pathlib.Path(sys.argv[1]).read_text()
root = pathlib.Path(sys.argv[2])

def entity_block(name):
    start = context.index(f"modelBuilder.Entity<{name}>")
    return context[start:context.index("\n        });", start)]

def require(text, value):
    if value not in text:
        raise SystemExit(f"Schema contract failed: missing {value}")

for name, fields in {
    "POSem": [
        'entity.Property(e => e.Doku).HasMaxLength(50);',
        'entity.Property(e => e.Kode_Supplier).HasMaxLength(12);',
        'entity.Property(e => e.Kode_Valas).HasMaxLength(12);',
        'entity.Property(e => e.Kode_dept).HasMaxLength(12);',
        'entity.Property(e => e.Hapus).HasMaxLength(100);',
    ],
    "SubPOSem": [
        'entity.Property(e => e.Doku).HasMaxLength(50);',
        'entity.Property(e => e.Alias).HasMaxLength(20);',
        'entity.Property(e => e.Keterangan).HasMaxLength(255);',
        'entity.Property(e => e.Kode_Brg).HasMaxLength(50);',
        'entity.Property(e => e.Kode_Dept).HasMaxLength(12);',
        'entity.Property(e => e.Kode_Gudang).HasMaxLength(10);',
        'entity.Property(e => e.Kode_Valas).HasMaxLength(10);',
        'entity.Property(e => e.Model).HasMaxLength(255);',
        'entity.Property(e => e.Merk).HasMaxLength(100);',
        'entity.Property(e => e.Satuan).HasMaxLength(10);',
    ],
}.items():
    block = entity_block(name)
    for field in fields:
        require(block, field)

confirmation = (root / "Features/PurchaseOrders/Commands/ConfirmPurchaseOrderCommandHandler.cs").read_text()
confirmation_command = (root / "Features/PurchaseOrders/Commands/ConfirmPurchaseOrderCommand.cs").read_text()
confirmed_dto = (root / "Features/PurchaseOrders/DTOs/ConfirmedPurchaseOrderDtos.cs").read_text()
confirmed_detail = (root / "Features/PurchaseOrders/Queries/GetConfirmedPurchaseOrderByIdQueryHandler.cs").read_text()
confirmed_list = (root / "Features/PurchaseOrders/Queries/GetConfirmedPurchaseOrdersQueryHandler.cs").read_text()
gr_line_item = (root / "Features/GoodsReceipts/DTOs/GRLineItemDto.cs").read_text()
for invariant in [
    "line.id_sub_posem",
    "Duplicate draft line",
    "JumlahKonfirm = line.Jumlah",
    "draft.DPPNilaiLain is > 0",
    "KodeRnd = draftLine.KodeRnd",
    "ContactPr = request.ContactPr ?? draft.ContactPr",
    "Tgl_Pengiriman = request.Psd ?? draft.Tgl_Pengiriman",
    "TglShip = request.Etd ?? draft.TglShip",
    "if (draft.STS != \"0\")",
    "var discount = sourceQty == 0 ? 0 : (draftLine.Diskon ?? 0) * line.Jumlah / sourceQty",
    "var hasAbsolutePpn = sourcePpn > 100",
    "var confirmationBasis",
    "var sourceBasis",
    "var priorConfirmations = await _context.POs",
    "var finalConfirmation",
    "var allocatedDiskon = finalConfirmation",
    "var allocatedDpp = finalConfirmation",
    "var allocatedPpnTunai = finalConfirmation",
    "var allocatedPpn = finalConfirmation",
    "Diskon = allocatedDiskon",
    "DPPNilaiLain = sourceDpp > 0 ? allocatedDpp",
    "PPnTunai = allocatedPpnTunai",
    "PPN = storedPpn",
    "var vat = hasAbsolutePpn ? allocatedPpn",
]:
    require(confirmation, invariant)

for field in ["string? ContactPr", "DateTime? Psd", "DateTime? Etd"]:
    require(confirmation_command, field)

goods_receipt = (root / "Features/GoodsReceipts/Commands/CreateGoodsReceiptCommandHandler.cs").read_text()
require(goods_receipt, "does not match item")
require(goods_receipt, "requestedQtyByItem")
require(goods_receipt, "ambiguousSku")
require(goods_receipt, "item.ResolvedSubPOId")

for value in ["string? Doku_POSem,", 'JsonPropertyName("id_sub_po_confirmation")']:
    require(confirmed_dto, value)
require(confirmed_detail, "row.Doku_POSem")
require(confirmed_list, "Doku_PO = po.Doku ?? string.Empty")
require(gr_line_item, 'JsonPropertyName("id_sub_po_confirmation")')

for path in [
    root / "Features/PurchaseOrders/Queries/GetConfirmedPurchaseOrdersQueryHandler.cs",
    root / "Features/PurchaseOrders/Queries/GetConfirmedPurchaseOrderByIdQueryHandler.cs",
]:
    if not path.exists():
        raise SystemExit(f"Schema contract failed: missing {path.name}")

require(
    (root / "Features/PurchaseOrders/Queries/GetConfirmedPurchaseOrdersQueryHandler.cs").read_text(),
    "Doku = po.Doku ?? string.Empty,",
)
PY
