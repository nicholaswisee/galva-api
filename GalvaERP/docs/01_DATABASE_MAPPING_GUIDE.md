# 01. Database & Entity Mapping Guide

> **Source of Truth:** `galva-db/schema.sql` (`ErpApMockup` SQL Server 2022 DB).
> This document maps C# EF Core entities in `galva-api` to the database schema.

---

## 1. P2P Table Catalog & Entity Mapping

The database uses Indonesian ERP table names. The table below lists the EF Core Entity, DB Table name, Primary Key, and purpose across the P2P lifecycle:

| Domain Feature | C# Entity Class | DB Table Name | Primary Key Column | PK Type | Notes |
|---|---|---|---|---|---|
| Purchase Requisition Header | `SPB` | `[dbo].[SPB]` | `id_spb` | `bigint IDENTITY` | Requisition header. Header filter: `Hapus IS NULL`. |
| Purchase Requisition Detail | `SubSPB` | `[dbo].[SubSPB]` | `id_sub_spb` | `bigint IDENTITY` | Requisition line items. Contains computed column `Jumhar`. |
| Purchase Order (Draft) Header | `POSem` | `[dbo].[POSem]` | `id_posem` / `Doku` | — | Initial draft Purchase Order issued to vendor. |
| Purchase Order (Draft) Detail | `SubPOSem` | `[dbo].[SubPOSem]` | `id_sub_posem` / `Doku` | — | Initial draft PO line items. |
| PO Confirmation Header | `PO` | `[dbo].[PO]` | `id_po` | `bigint IDENTITY` | Finalized supplier-confirmed PO. Header filter: `Hapus IS NULL`. Has `Doku_POSem` reference. |
| PO Confirmation Detail | `SubPO` | `[dbo].[SubPO]` | `id_sub_po` | `bigint IDENTITY` | Confirmed PO line items. Tracks `JumlahKonfirm`. |
| Goods Receipt Header | `LPB` | `[dbo].[LPB]` | `id_lpb` | `bigint IDENTITY` | Received inventory/stock header. Linked to `PO.Doku`. |
| Goods Receipt Detail | `SubLPB` | `[dbo].[SubLPB]` | `id_sub_lpb` | `bigint IDENTITY` | Goods receipt line items. Links back to `SubPO`. |
| AP Invoice Header | `VoucherAP` | `[dbo].[VoucherAP]` | `PKbas` | `bigint IDENTITY` | Account Payable invoice header. Linked via `Doku_LPB` or `Doku_PO`. |
| AP Invoice Detail | `SubVoucherAP` | `[dbo].[SubVoucherAP]` | `PKbas` | `bigint IDENTITY` | Account Payable invoice lines. Constrained by `CK_SubVoucherAP_TipeBiaya`. |
| Payment Header | `Bayar` | `[dbo].[Bayar]` | `PKindex` | `bigint IDENTITY` | Vendor payment header. |
| Payment Detail | `SubBayar` | `[dbo].[SubBayar]` | `PKbas` | `bigint IDENTITY` | Vendor payment line items referencing `VoucherAP.Doku`. |

---

## 2. Master Data Entities Mapping

| C# Entity Class | DB Table Name | Primary Key | Key Columns | Notes |
|---|---|---|---|---|
| `Supplier` | `[dbo].[Supplier]` | `id_supplier` (`bigint IDENTITY`) | `Kode`, `Nama`, `KodeEPK`, `KodeGTC` | Vendor directory. |
| `Dept` | `[dbo].[Dept]` | `id_dept` (`bigint IDENTITY`) | `Kode`, `Nama` | Internal departments and approval roles. |
| `Gudang` | `[dbo].[Gudang]` | `id_gudang` (`bigint IDENTITY`) | `Kode`, `Nama`, `Kode_Area` | Warehouse facilities. |
| `Bank` | `[dbo].[Bank]` | `PKindex` (`bigint IDENTITY`) | `Kode`, `Nama`, `AC`, `AN` | Bank accounts for payments. |
| `Category` | `[dbo].[Category]` | `id_category` (`bigint IDENTITY`) | `Kode`, `Nama` | Product category taxonomy. |
| `Satuan` | `[dbo].[Satuan]` | `id_satuan` (`bigint IDENTITY`) | `Kode`, `Nama` | Units of measure (UOM). |
| `Sales` | `[dbo].[Sales]` | `id_sales` (`bigint IDENTITY`) | `Kode`, `Nama` | Sales person reference. |
| `Area` | `[dbo].[Area]` | `id_area` (`bigint IDENTITY`) | `Kode`, `Nama` | Geographic regions. |
| `Barang` | `[dbo].[Barang]` | **No PK (Loose)** | `Kode`, `Nama`, `Merk`, `Satuan`, `Harga` | Product items/SKUs. Joined on `Kode`. |

---

## 3. Entity Framework Core Configuration (`AppDbContext.cs`)

### 3.1 Key Entity Configurations

```csharp
// Example Fluent Configuration in AppDbContext.OnModelCreating
builder.Entity<SPB>(entity =>
{
    entity.ToTable("SPB");
    entity.HasKey(e => e.id_spb);
    entity.Property(e => e.RowVersion).IsRowVersion();
    entity.HasQueryFilter(e => e.Hapus == null);
});

builder.Entity<SubSPB>(entity =>
{
    entity.ToTable("SubSPB");
    entity.HasKey(e => e.id_sub_spb);
    entity.Property(e => e.Jumhar).ValueGeneratedOnAddOrUpdate(); // Computed column!
});

builder.Entity<PO>(entity =>
{
    entity.ToTable("PO");
    entity.HasKey(e => e.id_po);
    entity.Property(e => e.RowVersion).IsRowVersion();
    entity.HasQueryFilter(e => e.Hapus == null);
});

builder.Entity<SubPO>(entity =>
{
    entity.ToTable("SubPO");
    entity.HasKey(e => e.id_sub_po);
});

builder.Entity<VoucherAP>(entity =>
{
    entity.ToTable("VoucherAP");
    entity.HasKey(e => e.PKbas);
    entity.Property(e => e.RowVersion).IsRowVersion();
});

builder.Entity<SubVoucherAP>(entity =>
{
    entity.ToTable("SubVoucherAP");
    entity.HasKey(e => e.PKbas);
});

builder.Entity<Barang>(entity =>
{
    entity.ToTable("Barang");
    entity.HasNoKey(); // No primary key in database schema
});
```

---

## 4. Critical Database Schema Gotchas

### Gotcha 1: `SubSPB.Jumhar` Computed Column
`SubSPB.Jumhar` is defined in `schema.sql` as a computed column:
```sql
[Jumhar] AS (([jumlah]*[Harga]))
```
- **Rule:** Do NOT specify values for `Jumhar` in EF Core `INSERT` or `UPDATE` statements.
- EF Core entity mapping MUST set `.ValueGeneratedOnAddOrUpdate()` or omit `Jumhar` from insert properties.

### Gotcha 2: SQL Server `RowVersion` / `timestamp`
- `RowVersion` columns (`SPB.RowVersion`, `PO.RowVersion`, `LPB.RowVersion`, `VoucherAP.RowVersion`, `Bayar.RowVersion`) are auto-managed 8-byte binary tokens by SQL Server.
- Never pass manual values for `RowVersion` when creating or modifying records.
- Surfaced via HTTP `ETag` header as Base64 string.

### Gotcha 3: Foreign Key Constraints & Loose Relations
- There are **NO foreign key constraints** defined in `schema.sql`.
- Cross-table linkages (e.g. `SubPO.Doku -> PO.Doku`, `SubLPB.Doku_PO -> PO.Doku`, `SubBayar.Doku_Faktur -> VoucherAP.Doku`) rely entirely on string document numbers (`nvarchar`).
- Application handlers MUST explicitly validate referential integrity before persisting.

### Gotcha 4: Loose Master Data (`Barang`)
- `Barang` table does NOT have a primary key column in `schema.sql`.
- Queries joining `Barang` must join on `Kode` (`nvarchar(50)`).

### Gotcha 5: Soft Delete Pattern (`Hapus`)
- Database rows are soft-deleted by setting the `Hapus` column (e.g. `"Y"` or `"username|timestamp"`).
- Active queries must include `WHERE Hapus IS NULL` or rely on EF Core Global Query Filters.
