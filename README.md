# galva-api

ASP.NET Core 8 backend for the Galva ERP Account-Payable mockup. Sits
behind the [PWA client](../galva-client) and reads / writes the
[`ErpApMockup`](../galva-db) SQL Server 2022 database running in Docker.

It exposes the full P2P flow as REST endpoints:

```
Auth  →  Master Data  →  Purchase Requisitions (SPB)  →  Purchase Orders (PO)
                       →  Goods Receipts (LPB)        →  AP Invoices (VoucherAP)
                       →  Payments (Bayar)            →  Web Push
```

The actual .NET solution and project live in
[`GalvaERP/`](./GalvaERP) — open
[`GalvaERP/README.md`](./GalvaERP/README.md) for the full reference
(endpoints, auth, idempotency, ETag, CORS, configuration, project
layout, and known quirks).

## Repo layout

```
galva-api/
├── GalvaERP/
│   ├── GalvaERP.sln                          Solution file
│   └── GalvaERP/                             The .NET project (this is the project root)
│       ├── GalvaERP.csproj
│       ├── Program.cs                        Composition root: services, middleware, routes
│       ├── appsettings.json                  Committed config (safe values only)
│       ├── appsettings.Development.json
│       ├── Common/                           Behaviors, exceptions, middleware, push, security
│       ├── Domain/Entities/                  EF Core entity classes
│       ├── Infrastructure/Data/              AppDbContext
│       └── Features/                         Vertical slices (one folder per area)
└── README.md                                 (this file)
```

## Quick start

```bash
# 1. Make sure the DB is up — see ../galva-db/README.md → "Quick start"
#    The API expects a SQL Server at localhost:1433, database
#    ErpApMockup, user sa, password GalvaDev2026_StrongPwd.

# 2. Set up user-secrets (JWT signing key + VAPID keys) — one-time
cd GalvaERP                                 # the .csproj folder
dotnet user-secrets init

# Generate both VAPID keys in ONE call (running it twice produces a mismatched pair)
VAPID_OUTPUT=$(npx web-push generate-vapid-keys)
PUBLIC_KEY=$(echo "$VAPID_OUTPUT" | sed -n 's/^Public Key:$//p'  | tr -d '[:space:]')
PRIVATE_KEY=$(echo "$VAPID_OUTPUT" | sed -n 's/^Private Key:$//p' | tr -d '[:space:]')

dotnet user-secrets set "Jwt:SecretKey"     "$(openssl rand -base64 48)"
dotnet user-secrets set "VAPID:PublicKey"   "$PUBLIC_KEY"
dotnet user-secrets set "VAPID:PrivateKey"  "$PRIVATE_KEY"

# 3. Run
dotnet run
# → Now listening on: http://localhost:5132
# → /swagger  (OpenAPI UI in Development env only)
# → admin / admin123 is auto-seeded on first boot
```

| Command            | What it does                                            |
| ------------------ | ------------------------------------------------------- |
| `dotnet run`       | Build + start on `http://localhost:5132`.              |
| `dotnet build`     | Compile only.                                           |
| `dotnet watch run` | Hot-reload on file changes.                             |
| `dotnet test`      | Run unit / integration tests (none yet).                |

## Tech stack

| Concern     | Choice                                                              |
| ----------- | ------------------------------------------------------------------- |
| Runtime     | .NET 8 (`net8.0`), C# 12                                           |
| Web         | ASP.NET Core 8 minimal hosting + endpoint routing                   |
| ORM         | Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.SqlServer`) |
| Validation  | FluentValidation 12 with a `ValidationBehaviour` MediatR pipeline  |
| CQRS        | MediatR 14 — Commands + Queries + Handlers per feature              |
| Auth        | `Microsoft.AspNetCore.Authentication.JwtBearer` + BCrypt.Net-Next  |
| Web push    | `WebPush` library + VAPID keys from config                          |
| API docs    | Swashbuckle (`Swagger` + `SwaggerUI`) at `/swagger`                 |
| Observability | Sentry (`Sentry.AspNetCore`)                                      |
| IDE         | JetBrains Rider / VS 2022                                          |

## Endpoints (TL;DR)

All endpoints (except `POST /api/auth/login`, `POST /api/auth/refresh`,
`POST /api/auth/logout`, and `GET /api/push/vapid-public-key`) require
a valid JWT access token. The full list — with status codes and
request/response shapes — is in the Swagger doc at
`http://localhost:5132/swagger` once the API is running.

| Group                | Routes                                                                |
| -------------------- | --------------------------------------------------------------------- |
| **Auth**             | `POST /api/auth/login`, `/refresh`, `/logout`                         |
| **MasterData**       | `GET /api/master-data/{vendors,departments,inventory,warehouses,banks}` |
| **PurchaseRequisitions** | `GET/POST /api/purchase-requisitions`, `GET/PUT /api/purchase-requisitions/{doku}`, `POST /api/purchase-requisitions/{doku}/verify` |
| **PurchaseOrders**   | `GET/POST /api/purchase-orders`, `GET/PUT/DELETE /api/purchase-orders/{doku}`, `POST /api/purchase-orders/{doku}/verify` |
| **POConfirmations**  | `GET/POST /api/po-confirmations`, `GET /api/po-confirmations/{doku}` |
| **GoodsReceipts**    | `GET/POST /api/goods-receipts`, `GET/PUT/DELETE /api/goods-receipts/{doku}`  |
| **Invoices**         | `GET/POST /api/invoices`, `POST /api/invoices/po-based`, `GET/PUT/DELETE /api/invoices/{doku}` |
| **Payments**         | `GET/POST /api/payments`, `GET/PUT /api/payments/{doku}`              |
| **PurchaseReturns**  | `GET/POST /api/purchase-returns`, `GET /api/purchase-returns/eligible-lines?doku_Faktur=…`, `GET/PUT/DELETE /api/purchase-returns/{doku}` |
| **Push**             | `GET /api/push/vapid-public-key`, `POST/DELETE /api/push/subscribe`, `POST /api/push/test` |

Status codes: 200 on success, 201 on create, 400 on validation, 401 on
missing/invalid token, 404 on missing document, 409 on
idempotency-key reuse with a different body, 412 on stale ETag, 428
if `If-Match` is missing on PUT, 422 on business-rule violation.

For the full reference — auth flow, idempotency middleware, ETag-based
concurrency, validation pipeline, configuration keys, CORS, project
layout, and known quirks — see
[`GalvaERP/README.md`](./GalvaERP/README.md).

## Related docs

- [Client README](../galva-client/README.md) — what the SPA expects from
  each endpoint and how it handles auth/idempotency.
- [Database README](../galva-db/README.md) — Docker setup, schema, and
  the seed-from-prod pipeline.
