# 00. Overview & System Architecture

> **Audience:** Developers and AI Agents.
> This document details the technical foundation, cross-cutting concerns, middleware pipeline, and system architecture of `galva-api`.

---

## 1. System Context & Tech Stack

`galva-api` is an ASP.NET Core 8 minimal-API backend providing a full Account-Payable / Purchase-to-Pay (P2P) REST API service for Galva ERP.

| Component | Technology | Details / Versons |
|---|---|---|
| Framework | ASP.NET Core 8 | `net8.0`, C# 12 minimal APIs |
| Database | SQL Server 2022 | Database: `ErpApMockup` |
| ORM | Entity Framework Core 8 | `Microsoft.EntityFrameworkCore.SqlServer` 8.0 |
| Pattern | CQRS / Vertical Slice | `MediatR` 12.4.1 |
| Validation | FluentValidation | `FluentValidation.DependencyInjectionExtensions` 11.9 |
| Auth | JWT Bearer | `Microsoft.AspNetCore.Authentication.JwtBearer` 8.0 |
| Password Hashing | BCrypt | `BCrypt.Net-Next` 4.0.3 |
| Push Notifications | WebPush | `WebPush` 1.0.12 (VAPID protocol) |
| Idempotency | Custom Middleware | SHA-256 POST body hashing via `Tx_IdempotencyRecord` |
| Concurrency | Optimistic | Base64 `RowVersion` (`timestamp`) surfaced as `ETag` / `If-Match` |

---

## 2. Directory & Vertical Slice Layout

The solution follows a domain-driven Vertical Slice Architecture under `GalvaERP/Features/`:

```
GalvaERP/
├── Program.cs                         # Application entrypoint & composition root
├── appsettings.json                   # Base configuration (JWT, Push, Connection Strings)
├── Common/
│   ├── Behaviors/ValidationBehaviour.cs  # MediatR validation pipeline
│   ├── Exceptions/                    # DomainException, NotFoundException, ConcurrencyException
│   ├── Middleware/
│   │   ├── CorrelationIdMiddleware.cs # X-Correlation-Id header management
│   │   └── IdempotencyMiddleware.cs   # POST request deduplication middleware
│   ├── Push/                          # VAPID PushService and subscriptions
│   ├── Security/                      # JwtTokenGenerator, BCrypt password utilities
│   └── Web/RouteParams.cs             # Route parameter URL-decoding helpers ({*doku})
├── Domain/Entities/                   # EF Core domain entity models (matching schema.sql)
├── Infrastructure/Data/AppDbContext.cs # EF Core DbContext with 31+ DbSets and fluent mappings
└── Features/                          # Feature slices (Commands, Queries, DTOs, Endpoints)
    ├── Auth/
    ├── MasterData/
    ├── PurchaseRequisitions/          # SPB / SubSPB
    ├── PurchaseOrders/                # POSem / SubPOSem (Draft POs) & PO / SubPO (PO Confirmations)
    ├── GoodsReceipts/                 # LPB / SubLPB
    ├── Invoices/                      # VoucherAP / SubVoucherAP
    ├── Payments/                      # Bayar / SubBayar
    └── Push/                          # Web Push registration endpoints
```

---

## 3. Cross-Cutting Infrastructure Concerns

### 3.1 Idempotency Middleware (`IdempotencyMiddleware.cs`)
- Intercepts non-GET HTTP requests (`POST`) that carry an `Idempotency-Key` header (or auto-hashes POST body if header is omitted).
- Computes SHA-256 hash of the request body and checks `Tx_IdempotencyRecord` table.
- If a matching request key is found:
  - If completed, returns cached status code and response payload immediately.
  - If processing, returns `409 Conflict` ("Request with this Idempotency-Key is currently processing").
- Records completion and status upon success.

### 3.2 Optimistic Concurrency Control (`ETag` & `If-Match`)
- Every transaction header table in `schema.sql` (`SPB`, `PO`, `LPB`, `VoucherAP`, `Bayar`, etc.) includes a `[RowVersion] [timestamp] NOT NULL` column.
- EF Core surfaces `RowVersion` as a byte array (`byte[]`) with `[Timestamp]` or `.IsRowVersion()`.
- API endpoints map `RowVersion` to a Base64 string returned in the `ETag` HTTP response header:
  `ETag: "QkNE...=="`
- `PUT` and `DELETE` requests MUST supply an `If-Match` header containing the Base64 ETag.
- Handlers attach the original `RowVersion` byte array to `DbContext.Entry(entity).Property(e => e.RowVersion).OriginalValue`.
- If another process modified the row, EF Core throws `DbUpdateConcurrencyException`, mapped by the API to `412 Precondition Failed`.

### 3.3 Route Decoding Helper (`RouteParams.cs`)
- Document numbers (`Doku`) contain slashes (e.g. `SPB/2026/07/001` or `PO-20260730-001`).
- Endpoint routes use catch-all route parameters: `app.MapGet("/{*doku}", ...)`
- In handlers, raw route values are decoded using `RouteParams.Decode(doku)` (`Uri.UnescapeDataString`).

### 3.4 Soft Deletes (`Hapus`)
- Transactions are NEVER physically deleted using SQL `DELETE`.
- Soft delete is triggered by populating the `Hapus` column (storing string username/timestamp, e.g. `"Y"` or `"admin|2026-07-30"`).
- All queries across feature slices filter out deleted records:
  `WHERE p.Hapus == null`

---

## 4. Middleware Execution Order

```
Request Received
  │
  ├── CorrelationIdMiddleware    (Injects / echoes X-Correlation-Id)
  ├── IdempotencyMiddleware      (Deduplicates POST operations)
  ├── Authentication & JwtBearer (Enforces Bearer tokens)
  ├── ValidationBehaviour        (MediatR FluentValidation pipeline)
  └── Minimal API Endpoint Handler
```
