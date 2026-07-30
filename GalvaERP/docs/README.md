# GalvaERP API — Agentic Documentation Suite

> **For AI Agents and Developers.** This directory contains the authoritative, modular documentation suite for the **GalvaERP API** (`galva-api`). It details the system architecture, database-to-entity mappings based on the latest `galva-db` (`schema.sql`), Purchase-to-Pay (P2P) business logic, REST endpoint specifications, and developer/agent guidelines.

---

## 📚 Documentation Index

| File | Description | Key Focus Areas |
|---|---|---|
| [**00_OVERVIEW_AND_ARCHITECTURE.md**](file:///home/nicholaswisee/Code/projects/galva/galva-api/GalvaERP/docs/00_OVERVIEW_AND_ARCHITECTURE.md) | Architectural Foundation | ASP.NET Core 8, MediatR CQRS, Idempotency, Optimistic Concurrency (ETags), Error Handling |
| [**01_DATABASE_MAPPING_GUIDE.md**](file:///home/nicholaswisee/Code/projects/galva/galva-api/GalvaERP/docs/01_DATABASE_MAPPING_GUIDE.md) | Schema & Entity Mapping | Ground-truth mappings between EF Core entities and `schema.sql`, PK types, `RowVersion`, Gotchas |
| [**02_P2P_BUSINESS_LOGIC_GUIDE.md**](file:///home/nicholaswisee/Code/projects/galva/galva-api/GalvaERP/docs/02_P2P_BUSINESS_LOGIC_GUIDE.md) | Business Lifecycle & Rules | Requisitions (`SPB`) → POs (`POSem`) → PO Confirmations (`PO`) → Goods Receipts (`LPB`) → AP Invoices (`VoucherAP`) → Payments (`Bayar`) |
| [**03_API_ENDPOINTS_SPECIFICATION.md**](file:///home/nicholaswisee/Code/projects/galva/galva-api/GalvaERP/docs/03_API_ENDPOINTS_SPECIFICATION.md) | REST Endpoint Specs | Complete endpoint contracts, request/response DTO payloads, route decoding, header requirements |
| [**04_AGENTIC_DEVELOPMENT_GUIDE.md**](file:///home/nicholaswisee/Code/projects/galva/galva-api/GalvaERP/docs/04_AGENTIC_DEVELOPMENT_GUIDE.md) | Agentic Playbook & Best Practices | Step-by-step instructions for adding features, defensive coding rules, non-obvious gotchas for AI agents |

---

## 🚀 Quick Reference Summary

- **Runtime & Framework:** .NET 8 (`net8.0`), C# 12, ASP.NET Core 8 Minimal APIs.
- **Database:** SQL Server 2022 (`ErpApMockup`). Database schema defined in `galva-db/schema.sql`.
- **Architectural Style:** Vertical Slice Architecture using MediatR (CQRS).
- **Concurrency Control:** SQL Server `RowVersion` (`timestamp`) surfaced as Base64 HTTP `ETag` / `If-Match` headers.
- **Idempotency:** Custom `IdempotencyMiddleware` using SHA-256 body hashing stored in `Tx_IdempotencyRecord`.
- **Soft Delete:** Logical soft-delete via `Hapus` column (`WHERE Hapus IS NULL`).
