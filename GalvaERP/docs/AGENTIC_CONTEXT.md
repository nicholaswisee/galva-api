# Galva ERP API — Agentic Knowledge Bank

> **Historical context only.** The current API contract lives in
> [`../README.md`](../README.md) and the live `/swagger` document; the database
> terminology lives in [`../../../galva-db/docs/AGENTIC_CONTEXT.md`](../../../galva-db/docs/AGENTIC_CONTEXT.md).
> This audit predates the restored `POSem` and payment tables, so do not treat
> its historical schema-drift claims as authoritative.
>
> **For autonomous agents.** Read this before touching any endpoint, handler,
> DTO, or entity in `galva-api`. This document is the primary source of truth
> for request flow, business rules, naming conventions, non-obvious facts,
> gotchas, and the audit history that tracks schema drift from the database.
>
> The database schema and its terminology live in
> [`../../../galva-db/docs/AGENTIC_CONTEXT.md`](../../../galva-db/docs/AGENTIC_CONTEXT.md)
> — that document is the **canonical P2P source of truth**. Any API code,
> DTO, or in-repo doc that disagrees with it is wrong and should be fixed.

---

## 1. Module purpose

`galva-api` is an ASP.NET Core 8 (.NET 8 / C# 12) REST backend that exposes the
**Purchase-to-Pay (P2P)** flow of the `ErpApMockup` SQL Server 2022 mock-up
database (see `galva-db`). The HTTP surface is JWT-protected; clients are the
React PWA in `galva-client` (and `/swagger` in development).

Stack: minimal-API endpoints + MediatR 14 CQRS + EF Core 8 (no migrations —
schema lives in `galva-db/schema.sql`) + FluentValidation 12 + JWT bearer +
BCrypt + WebPush (VAPID) + Sentry. The full reference is in
[`../README.md`](../README.md).

---

## 2. Canonical P2P Flow (truth-aligned with `galva-db`)

The API contracts must match the canonical P2P chain documented by the
database. Tables are linked by **document numbers (`[Doku]`) — there are no
foreign keys** (galva-db §7 fact 2); the API enforces referential integrity.

```
Purchase Requisition   SPB / SubSPB                       (header + lines)
        │
        ▼
PO Confirmation         PO / SubPO                         (the supplier-confirmed, finalized PO)
        │
        ▼
Goods Receipt           LPB / SubLPB                       (physical intake at warehouse)
        │
        ▼
AP Invoice              VoucherAP / SubVoucherAP           (linked to LPB or directly to PO, via TipeBiaya)
        │
        ▼
Payment                 Bayar / SubBayar                   (SubBayar.Doku_Faktur → VoucherAP.Doku)
        │
        ▼
Purchase Return         ReturBeli / SubReturBeli           (optional branch off GR/Payment)
AR Receipt Note         TandaTerimaAr / SubTandaTerimaAr
```

> **⚠️ `POSem` is MISSING from `ErpApMockup` — known schema-parity gap, not a
> design choice.** Production has `POSem`/`SubPOSem` (the draft "sementara"
> PO) as real tables; the local schema has **no `CREATE TABLE [dbo].[POSem]`**
> (verified in `galva-db/schema.sql`, full git history, and the live container
> `sys.tables`). The Jul-29 schema sync mapped prod `POSem` rows **into**
> `PO`/`SubPO` (data-level remap) instead of creating the tables, and the
> `Doku_POSem` trace columns (`schema.sql:549, :1011`) were left NULL in the
> seed. The user's stated intent is local schema ≈ remote schema, so this is a
> **known incomplete sync**: create `POSem`/`SubPOSem` in `schema.sql`
> (DDL must come from prod — extraction scripts were deleted from `/tmp`),
> re-seed with `Doku_POSem` populated, and re-home `Features/POConfirmations`
> per §6.2. Until then: treat "no POSem stage" as a gap, `PO.STS = "0"` as the
> draft state on the single `PO` row, and older docs' `POS-yyyyMMdd` draft
> prefix as doc-only drift — the API only emits `PO-yyyyMMdd`.

### ⚠️ CRITICAL NAMING GOTCHA — `PO` vs the local `POConfirmation` staging tables

The ERP inverts what an outsider expects. **Always use this mapping**:

| DB Table | Business Name | Stage | Local? |
|---|---|---|---|
| `PO` / `SubPO` | PO Confirmation (the supplier-confirmed, finalized PO) | The only PO-shaped stage in the local mockup | ✅ Present locally |
| `POConfirmation` / `SubPOConfirmation` | Local-only staging mirror | **NOT a canonical flow step** | ⚠️ Present locally, prod-faithful legacy — see §6.2 |

**Consequences for the API** (full audit in §10):
- The "supplier confirmation" business action mutates the canonical `PO`/`SubPO`
  columns (`PO.STS` flips `"0"`→`"1"` on full confirm; `SubPO.JumlahKonfirm`
  accumulates). Per `galva-db` §2 and §14 the canonical tables themselves ARE
  the confirmation; the local-only `POConfirmation`/`SubPOConfirmation` staging
  tables must not be treated as a real flow step.
- `Features/POConfirmations/` today implements the supplier confirmation over
  those staging tables (see §6.2 for the **legacy-status** warning and the
  re-home TODO). `Features/PurchaseOrders/` has **no confirm verb of its own**;
  it cannot transition a PO to Confirmed directly.

---

## 3. Project Layout (the vertical-slice shape)

```
galva-api/GalvaERP/GalvaERP/                  ← the .NET project (this is the project root)
├── Program.cs                                Composition root: services, middleware, route map calls
├── GalvaERP.csproj                            Package refs + UserSecretsId
├── appsettings.json / appsettings.Development.json
├── Common/
│   ├── Behaviors/ValidationBehaviour.cs      MediatR pipeline: runs FluentValidation
│   ├── Exceptions/                            NotFoundException(→404), ConcurrencyException(→412),
│   │                                          DomainException(→422/409), ValidationException(→400)
│   ├── Middleware/                            CorrelationIdMiddleware, IdempotencyMiddleware (POST dedup)
│   ├── Push/                                  IPushService, PushService, VapidOptions (Web Push)
│   └── Security/                              JwtOptions, PasswordHasher (BCrypt 11), TokenService,
│                                              CurrentUser, AdminUserSeeder (seeds admin/admin123)
├── Domain/Entities/                          EF Core entity classes — ~35, one per DB table
├── Infrastructure/Data/AppDbContext.cs       31 DbSets + per-entity fluent config (key, table name,
│                                              max-lengths, RowVersion concurrency tokens)
└── Features/                                 Vertical slices, one folder per area:
    ├── Auth/                  Commands/ DTOs/ Endpoints/AuthEndpoints.cs
    ├── MasterData/           MasterDataEndpoints.cs
    ├── PurchaseRequisitions/ Commands/ DTOs/ Queries/ PurchaseRequisitionEndpoints.cs
    ├── PurchaseOrders/       Commands/ DTOs/ Queries/ PurchaseOrderEndpoints.cs
    ├── POConfirmations/      Commands/ DTOs/ Queries/ POConfirmationEndpoints.cs   ← legacy staging, §6.2
    ├── GoodsReceipts/        Commands/ DTOs/ Queries/ GoodsReceiptEndpoint.cs
    ├── Invoices/             Commands/ DTOs/ Queries/ APInvoiceEndpoints.cs
    ├── Payments/             Commands/ DTOs/ Queries/ PaymentEndpoints.cs           ← see §11 audit
    └── Push/                 Commands/ DTOs/ PushEndpoints.cs
```

Each `Features/<Area>/` follows the same shape:

- `Endpoints/<Area>Endpoints.cs` — minimal-API route registration; status-code
  mapping; **`If-Match` / ETag header handling** on mutating routes.
- `Commands/Create<Area>Command.cs` (+ validator + handler) and an
  `Update<Area>Command.cs` triplet — commands return the `Doku`.
- `Queries/Get<Area>sQuery.cs` (list) and `Get<Area>ByIdQuery.cs` (detail) —
  handlers project to DTOs.
- `DTOs/` — `<Area>ListDto`, `<Area>DetailDto`, `<Area>DetailLineDto`,
  `<Area>LineItemDto`. Use immutable `record` except for DTOs carrying a
  computed ETag, which are `class` so the SDK can hydrate them.

---

## 4. Cross-Cutting Behaviors

### 4.1 Auth — JWT access token + httpOnly refresh cookie
- `POST /api/auth/login` returns `{ accessToken, expiresIn }` and sets an
  httpOnly `refreshToken` cookie (7-day, `SameSite=Strict`, `Secure` over HTTPS).
- Access token: HS256, 15 min, claims `NameIdentifier` (userId), `Name`
  (username), `Role`. Sent as `Authorization: Bearer <token>`.
- `Program.cs` registers a **fallback policy** requiring auth on every route;
  the only anonymous routes are `POST /api/auth/login`, `POST /api/auth/refresh`,
  `POST /api/auth/logout`, `GET /api/push/vapid-public-key`.

### 4.2 Idempotency (`Common/Middleware/IdempotencyMiddleware.cs`)
- POSTs that carry `Idempotency-Key` are deduped for 24h: method + path + body
  SHA-256 hashed and cached in `Tx_IdempotencyRecord`.
- Hash mismatch on a reused key → `409 Conflict`. Same hash → cached response
  replayed.

### 4.3 ETag / `If-Match` concurrency (the canonical pattern)
- Every read endpoint returns `ETag: "<base64 RowVersion>"`. SQL Server
  `RowVersion`/`timestamp` columns auto-increment on every write; EF Core maps
  them with `.IsRowVersion().IsConcurrencyToken()`.
- Mutating `PUT`/`DELETE` endpoints **must read the `If-Match` header**, base64-
  decode it to a `byte[]` `IfMatchRowVersion`, and pass it on the command:
  ```csharp
  if (!TryReadIfMatch(ctx, out var ifMatch, out var error)) return error!;
  var updated = await mediator.Send(body with { Doku = ..., IfMatchRowVersion = ifMatch }, ct);
  ```
- Status codes: `428 Precondition Required` if header missing, `400` if not
  base64, `412 Precondition Failed` if `DbUpdateConcurrencyException` or an
  explicit `ConcurrencyException`.
- The `TryReadIfMatch` helper lives at the bottom of every endpoints file
  (copy-paste; not factored into `Common`). **Do not drift** — see §11 on the
  Payments feature for what happens when an endpoint forgets this pattern.

### 4.4 Validation (`Common/Behaviors/ValidationBehaviour.cs`)
- Every command has a sibling `*Validator` (FluentValidation). The pipeline
  runs all validators before the handler; failures become a `ValidationException`
  → `400` with a per-field error dictionary.
- **Validation-message prose style (mandatory per root README):** name the
  missing field in plain language, e.g. *"Vendor (Kode_Supplier) is required."*
  not *"Kode_Supplier is required."* This is now an enforced convention across
  PO, GR, and — per the audit in §11 — Payments.

### 4.5 Soft deletes (`Hapus`)
- `Hapus IS NULL` means active; a non-null value means logically deleted.
  Never `_context.Entities.Remove(...)` on ERP transaction tables.
- All "list" and "detail" queries on `Hapus`-bearing tables **must filter
  `Hapus IS NULL`** (galva-db §7 fact 16).
- Cancellation of a document is a soft delete (`Hapus` set), **not** a `STS`
  transition.

---

## 5. Document-Number Generation (`Doku`)

Handlers auto-generate document numbers; clients cannot supply their own. The
canonical format is `<PREFIX>-yyyyMMdd-{seq:000}` where `seq` is the next
zero-padded count of existing docs with the same prefix (today only). The
canonical prefixes are:

| Stage | Header table | Prefix | Example |
|---|---|---|---|
| Purchase Requisition | `SPB` | `SPB-` | `SPB-20260730-001` |
| PO Confirmation | `PO` | `PO-` | `PO-20260730-001` |
| Goods Receipt | `LPB` | `LPB-` | `LPB-20260730-001` |
| AP Invoice | `VoucherAP` | `VAP-` | `VAP-20260730-001` |
| **Payment** | `Bayar` | **`BAY-`** | `BAY-20260730-001` |

> ⚠️ The repo doc `02_P2P_BUSINESS_LOGIC_GUIDE.md` previously listed
> `PAY-` for payments. The audit (§11) found the implementation used `PAY-`
> and corrected it to the canonical `BAY-`. This doc and the payments handler
> now agree.
>
> There is **no `POS-yyyyMMdd` Draft-PO prefix in this schema or this API.**
> That prefix appears only in the four numbered docs in `galva-api/GalvaERP/docs/`,
> which describe a production-only `POSem`/`SubPOSem` "Draft PO" stage. The
> local `ErpApMockup` has no `POSem` table at all (see the parity-gap note in
> §2) — only the legacy `[Doku_POSem]` columns on `PO`/`SubPO` — so the
> "draft" state is `PO.STS = "0"` on the single `PO` row. If POSem tables are
> added for schema parity (see §2), revisit this.

---

## 6. Entity / Feature Notes (non-obvious)

### 6.1 Approval / Verify columns (`StsVerify`, `TglVerify`) — now wired

`galva-db` carries internal-verify columns on the requisition and PO headers:

- `SPB.TglVerify` (`smalldatetime`), `SPB.StsVerify` (`bit`).
- `PO.StsVerify` (`bool?`), `PO.TglVerify` (`DateTime?`).
- Also present per-line on `SubSPB`/`SubPO` as `JumlahVerify`/`JumlahVerifyTemp`
  (line-quantity verify fields) — currently unused by the API.

The audit (§12) found these were **declared in the entities, mapped by EF
convention, but never read or written by any feature**. The API has been
corrected so the verify/approval step is now exposed as
`POST /api/<area>/{doku}/verify` endpoints that flip `StsVerify = true` and
stamp `TglVerify = now`, and the verify columns are surfaced on the detail and
list DTOs of both areas.

"Approval" in this codebase therefore means:
1. **Internal PR approval** — `POST /api/purchase-requisitions/{doku}/verify`
   stamps `SPB.StsVerify = true` + `TglVerify`. The PR's `STS`/`Status` strings
   are separate display state; the verify columns are the durable audit
   signal.
2. **Internal PO approval** — `POST /api/purchase-orders/{doku}/verify` does
   the same on `PO`. (Distinct from the supplier confirmation in §6.2.)

### 6.2 `Features/POConfirmations/` — LEGACY STAGING FEATURE (needs re-homing)

This slice currently lives over the local-only `POConfirmation`/
`SubPOConfirmation` staging tables:

- Routes: `GET/POST/PUT/DELETE /api/po-confirmations/{*doku}`
  (`POConfirmationEndpoints.cs`, registered at `Program.cs:152`).
- Generate `PCF-yyyyMMdd-nnn` document numbers.
- Implement the **supplier confirmation**: enforce parent `PO.STS == "0"` on
  create/update/delete, accumulate `SubPO.JumlahKonfirm`, and flip
  `PO.STS` to `"1"` when all lines are fully confirmed.
- Side-effect the canonical `PO`/`SubPO` columns — the only code that
  transitions a PO to Confirmed.

Per `galva-db` §2/§6/§14 these staging tables are **local-only legacy** and
must not be treated as a canonical flow step. The re-home TODO is: move the
confirm verb onto `Features/PurchaseOrders/` (e.g. `POST …/{doku}/confirm`)
mutating `PO`/`SubPO` directly; stop writing `POConfirmation`/`SubPOConfirmation`;
and migrate the `GoodsReceipts` legacy linkage (`LPB.Doku_PCF`,
`SubLPB.id_sub_po_confirmation`) to `LPB.Doku_PO`/`SubLPB.Doku_PO`. Do **not**
remove `Doku_PCF`/`id_sub_po_confirmation` columns — `galva-db` says they are
legacy-but-keep.

Until that re-home lands, treat `Features/POConfirmations` as the sole
implementation of the supplier-confirmation step, but flag any new code as
touching legacy.

### 6.3 `Features/GoodsReceipts/` — partial-receipt logic

- A GR binds to a PO Confirmation (`PO.Doku`) at the header level and to
  specific SubPO lines at the line level.
- The handler aggregates all prior `SubLPB` for the same PO line and rejects
  if the new receipt would exceed `SubPO.JumlahKonfirm` (over-receipt guard).
- The legacy linkage to `POConfirmation`/`SubPOConfirmation`
  (`LPB.Doku_PCF`, `SubLPB.id_sub_po_confirmation`) is kept but should be
  removed when §6.2 is re-homed.

### 6.4 `Features/Invoices/` (`VoucherAP` / `SubVoucherAP`)

- `VoucherAP.TipeBiaya` is `CHECK`-constrained to `NULL OR IN ('LPB', 'PO')`
  (galva-db fact 13). The handler sets `TipeBiaya` to indicate whether the
  invoice was drawn from a Goods Receipt (`'LPB'`) or directly from a PO
  Confirmation (`'PO'`; canonical — see §6.2 caveat).
- `POST /api/invoices` (LPB-sourced) and `POST /api/invoices/po-based` are the
  two create endpoints.

### 6.5 `Features/Payments/` (`Bayar` / `SubBayar`) — see §11 audit

Payment lines reference the **AP invoice by document number** via
`SubBayar.Doku_Faktur` (`→ VoucherAP.Doku`). `SubBayar.Doku_LPB` is an
optional secondary reference to the originating GR. **Never** create a payment
line without `Doku_Faktur`, and **never** let a cumulative `TotalNilai` for a
given invoice exceed `VoucherAP.Nilai` (over-payment guard, §11.2).

---

## 7. Cross-Table Document-Number Join Map (API layer enforces these)

Without FKs in the DB, this is the wiring the API must hold together (port of
`galva-db` §8 plus the API-specific tuples):

```
SPB.Doku            → SubSPB.Doku                                    (PR header → PR lines)
SPB.Doku            → PO.Doku_SPPB                                   (PO → originating PR)   [if used]
PO.Doku             → SubPO.Doku                                     (PO header → PO lines)
PO.Doku             → LPB.Doku_PO, SubLPB.Doku_PO                    (GR → PO Confirmation)
PO.Doku             → VoucherAP.Doku_PO, SubVoucherAP.Doku_PO         (AP → PO)
PO.Doku             → SubBayar.Doku_PO   (if used)                    (payment line → PO)   [optional]
LPB.Doku            → SubLPB.Doku                                    (GR header → GR lines)
LPB.Doku            → VoucherAP.Doku_LPB, SubVoucherAP.Doku_LPB       (AP → GR)
LPB.Doku            → SubBayar.Doku_LPB                              (payment → GR)          [optional]
VoucherAP.Doku      → SubVoucherAP.Doku                              (AP header → AP lines)
VoucherAP.Doku      → SubBayar.Doku_Faktur                           (payment line → AP)     PRIMARY LINK
Bayar.Doku          → SubBayar.Doku                                  (payment header → lines)
```

Legacy local-only linkage (kept, do-not-remove per `galva-db` §6):
`POConfirmation.Doku → {LPB,SubLPB,VoucherAP,SubVoucherAP}.Doku_PCF` and
`SubLPB.id_sub_po_confirmation`.

---

## 8. Status State Machines (`STS` / `Sts`)

Status columns are short strings (`nvarchar(1)` on `Bayar`/`SubBayar`/
`LPB`/`PO`; `nvarchar(3)` on `VoucherAP`). Two distinct state machines coexist:

### 8.1 Document lifecycle `STS` (PO, LPB, Bayar, VoucherAP)

```
   ┌──────────────┐
   │  "0" Pending │  ← handlers always create at "0"; editable; soft-deletable
   └──────┬───────┘
          │ confirm / receive / approve / disburse
          ▼
   ┌──────────────┐
   │  "1" Active  │  ← supplier-confirmed PO (PO), partially received PO/LPB, paid Bayar
   └──────┬───────┘
          │ fully processed
          ▼
   ┌──────────────┐
   │  "2" Closed  │  ← fully received PO/LPB (NOT "cancelled" — see §8.2)
   └──────────────┘
```

The repo doc `02_P2P_BUSINESS_LOGIC_GUIDE.md` previously stated `"2"=Cancelled`
for PO and listed `"3"=Completed`. **The implemented PO machine is 0→1→2**
where `"2"` is **fully received** (set by `CreateGoodsReceiptCommandHandler`
when every SubPO line is fully received), not cancelled. Cancellation is a
soft delete (`Hapus="Y"`), never an `STS` transition. Correct this doc if you
see the old language.

### 8.2 `"9"` is soft-cancelled for the staging feature

`DeletePOConfirmationCommandHandler` sets `POConfirmation.STS = "9"` on its
soft-cancel. The `GoodsReceipts` create handler guards
`po.STS == "9"` expecting cancellation — but **nothing writes `"9"` onto
`PO.STS`** (that soft-cancel happens on the staging table, not on `PO`).
The guard is effectively dead for `PO`; it should be guarding against
`Hapus IS NOT NULL` (galva-db fact 16) instead.

### 8.3 Internal verify (`StsVerify` / `TglVerify`) — separate audit signal

`StsVerify` (bit) + `TglVerify` (datetime) compose the internal-approval audit
signal — orthogonal to `STS` lifecycle. See §6.1 and §12. The verify endpoints
do not touch `STS`; the lifecycle guard still applies (`UpdatePurchaseOrder`
blocks at `STS != "0"` separately).

---

## 9. Coding Patterns (Rigid Rules)

These rules are non-negotiable. Drift causes the bugs catalogued in §11/§12.

1. **Strict vertical slice.** Endpoints delegate to `IMediator.Send`; no
   business logic in the endpoint. Handlers live under `Features/<Area>/…`.
2. **Database single source of truth.** Confirm column names and types in
   `galva-db/schema.sql` and `Domain/Entities/` before writing any handler or
   query. Mind spelling variants (`Kode_dept` in `PO` vs `Kode_Dept` in `SPB`;
   `PPN` in `PO` vs `PPn` in `SPB`; `Bayar.STS` vs `SubBayar.Sts`).
3. **No computed-column writes.** Never assign `SubSPB.Jumhar`
   (`AS ([jumlah]*[Harga])`) in C# — EF/SQL Server rejects it. Remove the
   property from input DTOs; let SQL Server compute it.
4. **RowVersion is opaque.** Never insert/update it; EF Core sets it via
   `.IsRowVersion().IsConcurrencyToken()`. Expose it only as base64 ETag.
5. **`If-Match` on every PUT/DELETE.** Use the canonical `TryReadIfMatch`
   helper (copy from any other endpoints file). Header missing → 428; bad
   base64 → 400; concurrency fail → 412 (catch `DbUpdateConcurrencyException`
   and `ConcurrencyException`).
6. **Soft delete only.** Set `Hapus` (any non-null works; the codebase uses
   `"Y"`; `galva-db/docs/AGENTIC_CONTEXT.md` §14 mentions
   `"username|timestamp"` style — both acceptable). Never `Remove(...)`.
7. **List/detail queries filter `Hapus IS NULL`.** Always. This rule was
   violated by `GetPaymentsQueryHandler` and `GetPaymentByIdQueryHandler`
   before the §11 audit.
8. **Validator prose.** Name the human field: *"Vendor (Kode_Supplier) is
   required."* — never the bare technical name.
9. **`Doku` is catch-all.** Routes that take a Doku use `/{*doku}` and
   `RouteParams.Decode(doku)` because document numbers can contain `/`.
10. **`STS`/`Sts` field lengths.** `Bayar.STS` and `SubBayar.Sts` are
    `nvarchar(1)`. Validators must constrain updates to the canonical states
    (`"0"`, `"1"`, `"2"` for the lifecycle; `"9"` is staging-only). The Update
    validator on Payments now enforces this (§11.4).
11. **Schema agreement.** Input DTOs/commands **must include the canonical
    cross-table linkage columns**. For payments that is `SubBayar.Doku_Faktur`
    (primary), not just `Doku_LPB`. The §11 audit fixed this.

---

## 10. Endpoint Surface (current, post-audit)

All routes except anonymous auth/push-vapid require a JWT.

| Area | Routes | Notes |
|---|---|---|
| Auth | `POST /api/auth/login|refresh|logout` | JWT + refresh cookie |
| MasterData | `GET /api/master-data/{vendors,departments,inventory,warehouses,banks}` | Projections only |
| PurchaseRequisitions | `GET/POST /api/purchase-requisitions`, `GET/PUT /: {*doku}`, **`POST /: {*doku}/verify`** (§12) | Verify endpoint internal PR approval |
| PurchaseOrders | `GET/POST /api/purchase-orders`, `GET/PUT/DELETE /: {*doku}`, **`POST /: {*doku}/verify`** (§12) | Verify endpoint internal PO approval; no confirm verb here (legacy in §6.2) |
| POConfirmations | `GET/POST/PUT/DELETE /api/po-confirmations/{*doku}` | ⚠️ LEGACY staging; see §6.2 |
| GoodsReceipts | `GET/POST /api/goods-receipts`, `GET/PUT /: {*doku}` | Over-receipt guard via SubPO.JumlahKonfirm |
| Invoices | `GET/POST /api/invoices`, `POST /api/invoices/po-based`, `GET/PUT/DELETE /: {*doku}` | `TipeBiaya ∈ {LPB, PO}` |
| Payments | `GET/POST /api/payments`, `GET/PUT /: {*doku}` | §11 audit fixes |
| Push | `GET /api/push/vapid-public-key`, `POST/DELETE /api/push/subscribe`, `POST /api/push/test` | Web Push / VAPID |

Status codes: 200 success, 201 create, 400 validation, 401 unauth, 404 missing,
409 idempotency-key reuse with different body, **412 stale ETag**, **428
`If-Match` missing**, 422 business-rule violation, 409 conflict on soft-delete
guard violation.

---

## 11. Audit & Fix History — `Features/Payments/` (`Bayar` / `SubBayar`)

The previous implementation drifted from the canonical schema in five ways.
All five are fixed; the diffs are documented here so a future agent knows what
changed and why.

### 11.1 Document-number prefix — `PAY-` → `BAY-`

`CreatePaymentCommandHandler` previously generated `PAY-yyyyMMdd-nnn`. The
canonical prefix is `BAY-` (matches `Bayar`/Hiapt06 and the convention used
upstream). Fix: prefix becomes `BAY-`. Downstream clients reading payments by
document number must expect `BAY-`.

### 11.2 Missing `Doku_Faktur` linkage → over-payment guard

The previous `PaymentLineItemDto` had only `Doku_LPB` (GR doc) — but the
primary link from a payment line to an AP invoice is
`SubBayar.Doku_Faktur` (`→ VoucherAP.Doku`, galva-db §8 + fact 14). Fix:

- `PaymentLineItemDto` (create input) now carries `Doku_Faktur` (required) and
  keeps `Doku_LPB` (optional reference).
- `CreatePaymentCommandHandler` for each line:
  1. Loads the `VoucherAP` by `Doku == line.Doku_Faktur`; 404 if missing.
     If `VoucherAP.Kode_Supplier` does not match the payment header's
     `Kode_Supplier`, throws `DomainException` (supplier mismatch).
  2. Aggregates all prior `SubBayar.TotalNilai` (`WHERE Doku_Faktur = line.Doku_Faktur
     AND Hapus IS NULL`) and adds the new `line.TotalNilai`. If the cumulative
     exceeds `VoucherAP.Nilai`, throws `DomainException` with a full-readable
     message in the style of the GR over-receipt guard:
     `"Payment for invoice VAP-... exceeds outstanding balance. Invoice=..., Nilai=..., already-paid=..., remaining=..., requested=..."`.
  3. Writes `SubBayar.Doku_Faktur`, `SubBayar.Doku_LPB` (optional), `Nilai`,
     `TotalNilai`, plus currency/bank fields from the header.

### 11.3 `If-Match` header on PUT (was previously body `ETag`)

`UpdatePaymentCommand` previously took a body `ETag` string; the endpoint sent
the ETag in the body. The canonical pattern (used by Invoices, PurchaseOrders,
PurchaseRequisitions, POConfirmations) is the **`If-Match` header** carrying
the base64 RowVersion. Fix:

- `UpdatePaymentCommand` now takes `byte[] IfMatchRowVersion` (no body `ETag`).
- `PaymentEndpoints.PUT` uses the canonical `TryReadIfMatch` helper — 428 if
  missing, 400 if not base64, 412 on `DbUpdateConcurrencyException`.
- Handler applies the RowVersion via EF Core `OriginalValue`:

  ```csharp
  _context.Entry(bayar).Property(e => e.RowVersion).OriginalValue = request.IfMatchRowVersion;
  ```

### 11.4 `STS` state validation + prose-style validator messages

`UpdatePaymentCommand` previously accepted any string for `STS`. Fix: the
validator constrains it to `{ "0", "1", "2" }` (or null = unchanged) and emits
human-named messages. `CreatePaymentCommandValidator` now says
*"Vendor (Kode_Supplier) is required."* and
*"Each payment line must reference an AP invoice (Doku_Faktur)."*.

### 11.5 `Hapus IS NULL` filter on read queries

`GetPaymentsQueryHandler` and `GetPaymentByIdQueryHandler` previously returned
soft-deleted rows. Both now filter `Hapus IS NULL` (galva-db fact 16).

---

## 12. Audit & Fix History — Approval (`StsVerify` / `TglVerify`)

The audit found `StsVerify` and `TglVerify` declared in `Domain/Entities/`
(`SPB.cs:12,72`, `PO.cs:110,112`) and EF-mapped by convention (`AppDbContext`
`:567`,`:676`) but **never referenced by any feature**. The internal-approval
step was effectively un-modeled on the API.

Fix (now applied): an internal-verify step is exposed as a dedicated action per
area:

- `POST /api/purchase-requisitions/{*doku}/verify` —
  `VerifyPurchaseRequisitionCommand` / `…Handler` sets `SPB.StsVerify = true`
  and `SPB.TglVerify = DateTime.UtcNow`. Requires the `If-Match` header (412
  on stale ETag), 404 if the PR is missing, returns the refreshed PR detail
  with a new ETag.
- `POST /api/purchase-orders/{*doku}/verify` — same for `PO`.

Both verify actions are **independent of `STS`** — they stamp the durable
audit signal but do not flip lifecycle status. Lifetime stays governed by the
existing lifecycle endpoints (`Update`, `Delete`, and — for the PO side — the
legacy `POST /api/po-confirmations`).

Additionally `StsVerify` and `TglVerify` are now surfaced on the PR and PO
detail DTOs (and on `PRListDto`/`POListDto`) so clients can display approval
state.

This wiring matches the canonical schema (the columns are the truth) without
overloading `STS` and without inventing a separate `Approval` table — there
is no `Approval` table and none is planned.

---

## 13. What NOT to Do

- **Do not** insert/update `RowVersion` (let EF Core manage it).
- **Do not** assign computed columns (`SubSPB.Jumhar`).
- **Do not** add `NULL` after a computed column definition in DDL.
- **Do not** treat `POConfirmation`/`SubPOConfirmation` as a canonical flow
  step (galva-db §2/§14; §6.2 here).
- **Do not** hard-delete rows from any ERP table; use the `Hapus` pattern.
- **Do not** emit messages as bare field names — use the prose style.
- **Do not** accept concurrency tokens in the request body; use `If-Match`.
- **Do not** create a payment line without `Doku_Faktur`; never allow
  cumulative `SubBayar.TotalNilai` for an invoice to exceed `VoucherAP.Nilai`.
- **Do not** rely on the legacy Draft-PO `POS-yyyyMMdd` prefix — the code only
  emits `PO-yyyyMMdd`; "draft" = `STS == "0"` on the same row.
- **Do not** put real secrets in `appsettings.json` (use user-secrets /
  environment variables).
- **Do not** deploy with the default `admin / admin123` (gate the
  `AdminUserSeeder`, don't remove it from `Program.cs`).

---

## 14. Credentials and Connection

The API expects the dockerised SQL Server from `galva-db` on
`localhost:1433`, database `ErpApMockup`, user `sa`, password
`GalvaDev2026_StrongPwd` (from `galva-db/.env`). JWT signing key and VAPID
keys are kept in user-secrets (UserSecretsId `f225e738-2ff9-4f39-8928-9bf74dead68b`).
Never commit real secrets.

---

## 15. Related Docs

- [`galva-db/docs/AGENTIC_CONTEXT.md`](../../../galva-db/docs/AGENTIC_CONTEXT.md)
  — the canonical schema knowledge bank. **The authority for table/column
  facts.**
- [`../README.md`](../README.md) — high-level API overview, quick-start,
  config keys, CORS.
- [`00_OVERVIEW_AND_ARCHITECTURE.md`](./00_OVERVIEW_AND_ARCHITECTURE.md),
  [`01_DATABASE_MAPPING_GUIDE.md`](./01_DATABASE_MAPPING_GUIDE.md),
  [`02_P2P_BUSINESS_LOGIC_GUIDE.md`](./02_P2P_BUSINESS_LOGIC_GUIDE.md),
  [`03_API_ENDPOINTS_SPECIFICATION.md`](./03_API_ENDPOINTS_SPECIFICATION.md),
  [`04_AGENTIC_DEVELOPMENT_GUIDE.md`](./04_AGENTIC_DEVELOPMENT_GUIDE.md) — the
  fine-grained reference docs. Where they disagree with
  `galva-db/docs/AGENTIC_CONTEXT.md` or this file, the canonical-context
  version wins; treat the others as historical context.

> Doc-drift tracking: the four numbered docs in `galva-api/GalvaERP/docs/`
> still contain a few spec-vs-implementation gaps (Draft-PO `POS-` prefix, the
> `"2"=Cancelled` state-machine wording, `/api/auth/register` payload
> examples that don't match the actual route set). They are useful reference
> but should be reconciled against this file before relying on them.
