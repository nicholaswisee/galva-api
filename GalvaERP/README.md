# GalvaERP API

ASP.NET Core 8 backend for the Galva ERP Account-Payable mockup. Sits
behind the [PWA client](../../galva-client/README.md) and reads /
writes the [`ErpApMockup`](../../galva-db/README.md) SQL Server 2022
database running in Docker.

It exposes the full P2P flow as REST endpoints:

```
Auth  →  Master Data  →  Purchase Requisitions (SPB)  →  Purchase Orders (PO)
                          →  Goods Receipts (LPB)        →  AP Invoices (VoucherAP)
                          →  Payments (Bayar)            →  Web Push
```

## Tech stack

| Concern | Choice |
|---|---|
| Runtime | .NET 8 (`net8.0`), C# 12 |
| Web | ASP.NET Core 8 minimal hosting + endpoint routing |
| ORM | Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.SqlServer`) |
| Validation | FluentValidation 12 with a `ValidationBehaviour` MediatR pipeline |
| CQRS | MediatR 14 — Commands + Queries + Handlers per feature |
| Auth | `Microsoft.AspNetCore.Authentication.JwtBearer` + BCrypt.Net-Next for password hashing |
| Web Push | `WebPush` library + VAPID keys from config |
| API docs | Swashbuckle (`Swagger` + `SwaggerUI`) at `/swagger` |
| Observability | Sentry (`Sentry.AspNetCore`) |
| IDE | JetBrains Rider / VS 2022 (project files in `GalvaERP/`) |

## Quick start

```bash
# 1. Make sure the DB is up
#    (see ../../galva-db/README.md → "Quick start")
#    The API expects a SQL Server at localhost:1433, database
#    ErpApMockup, user sa, password GalvaDev2026_StrongPwd.

# 2. Set up user-secrets (JWT signing key + VAPID keys)
cd GalvaERP                        # the project folder with the .csproj
dotnet user-secrets init           # one-time
dotnet user-secrets set "Jwt:SecretKey"  "<a-32+-char random string>"
dotnet user-secrets set "VAPID:PublicKey"  "<base64url 88 chars>"
dotnet user-secrets set "VAPID:PrivateKey" "<base64url 44 chars>"

# 3. Run
dotnet run
# → Now listening on: http://localhost:5132
# → /swagger  (OpenAPI UI in Development env only)
# → admin / admin123 is auto-seeded on first boot
```

The connection string and other safe-to-commit settings live in
`appsettings.json`. The JWT secret and VAPID keys are kept in
[user-secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets)
so they never enter version control.

## Scripts

| Command | What it does |
|---|---|
| `dotnet run` | Build + start on `http://localhost:5132`. |
| `dotnet build` | Compile only. |
| `dotnet watch run` | Hot-reload on file changes. |
| `dotnet test` | Run unit / integration tests (none yet — see Roadmap). |

The launch profile is in `Properties/launchSettings.json`:
- `http` → `http://localhost:5132`, ASPNETCORE_ENVIRONMENT=Development
- `https` → `https://localhost:7267;http://localhost:5132` (needs an
  ASP.NET Core dev cert; `dotnet dev-certs https --trust` once)

## Project layout

```
GalvaERP/
├── Program.cs                       Composition root: services, middleware, routes
├── GalvaERP.csproj                  Package references, UserSecretsId
├── appsettings.json                 Committed config (safe values only)
├── appsettings.Development.json     Dev overrides
│
├── Common/
│   ├── Behaviors/
│   │   └── ValidationBehaviour.cs  MediatR pipeline: runs FluentValidation
│   ├── Exceptions/
│   │   ├── NotFoundException.cs        + handler → 404
│   │   ├── ConcurrencyException.cs     + handler → 412
│   │   ├── DomainException.cs          + handler → 422
│   │   └── ValidationException.cs      + handler → 400
│   ├── Middleware/
│   │   ├── CorrelationIdMiddleware.cs X-Correlation-Id round-trip
│   │   └── IdempotencyMiddleware.cs    POST body hash + DB cache
│   ├── Push/
│   │   ├── IPushService.cs / PushService.cs   VAPID + WebPush
│   │   └── VapidOptions.cs
│   └── Security/
│       ├── JwtOptions.cs                Strongly-typed "Jwt" section
│       ├── PasswordHasher.cs            BCrypt work factor 11
│       ├── TokenService.cs              JWT + refresh-token issuance
│       ├── CurrentUser.cs               ClaimsPrincipal helper
│       └── AdminUserSeeder.cs           Seeds admin / admin123 on first boot
│
├── Domain/Entities/                  EF Core entity classes (scaffold-style)
│   ├── Master_User.cs                Auth + refresh token
│   ├── Tx_IdempotencyRecord.cs       Idempotency middleware cache
│   ├── Tx_PushSubscription.cs        Web push registrations
│   ├── Supplier.cs, Barang.cs, Gudang.cs, Bank.cs, …
│   ├── PO.cs, SubPO.cs               Purchase orders + lines
│   ├── LPB.cs, SubLPB.cs             Goods receipts + lines
│   ├── VoucherAP.cs, SubVoucherAP.cs AP invoices + lines
│   ├── Bayar.cs, SubBayar.cs         Payments + lines
│   ├── SPB.cs, SubSPB.cs             Purchase requisitions + lines
│   └── (35 entities total — one per table in the schema)
│
├── Infrastructure/
│   └── Data/
│       └── AppDbContext.cs           31 DbSets + per-entity fluent config
│
└── Features/                         Vertical slices, one folder per area
    ├── Auth/             Commands/  DTOs/  Endpoints/AuthEndpoints.cs
    ├── MasterData/       MasterDataEndpoints.cs
    ├── PurchaseRequisitions/   Commands/  DTOs/  Queries/  PurchaseRequisitionEndpoints.cs
    ├── PurchaseOrders/   Commands/  DTOs/  Queries/  PurchaseOrderEndpoints.cs
    ├── GoodsReceipts/    Commands/  DTOs/  Queries/  GoodsReceiptEndpoints.cs
    ├── Invoices/         Commands/  DTOs/  Queries/  APInvoiceEndpoints.cs
    ├── Payments/         Commands/  DTOs/  Queries/  PaymentEndpoints.cs
    └── Push/             Commands/  DTOs/  PushEndpoints.cs
```

Each `Features/<X>` folder follows the same shape:

- `Endpoints/<X>Endpoints.cs` — minimal-API route registration,
  status-code mapping, ETag handling.
- `Commands/` — `CreateXCommand`, `UpdateXCommand`, validator,
  handler. Handlers return the Doku of the new/updated document.
- `Queries/` — `GetXListQuery`, `GetXByIdQuery`, handlers that
  project to DTOs.
- `DTOs/` — `XListDto`, `XDetailDto`, `XLineItemDto`. Records
  for immutable DTOs, classes for DTOs with computed ETag.

## Endpoints

All endpoints (except `POST /api/auth/login`, `POST /api/auth/refresh`,
`POST /api/auth/logout`, and `GET /api/push/vapid-public-key`) require
a valid JWT access token. The full list is in the Swagger doc at
`/swagger` once the API is running.

| Group | Routes |
|---|---|
| **Auth** | `POST /api/auth/login`, `POST /api/auth/refresh`, `POST /api/auth/logout` |
| **MasterData** | `GET /api/master-data/{vendors,departments,inventory,warehouses,banks}` |
| **PurchaseRequisitions** | `GET/POST /api/purchase-requisitions`, `GET/PUT /api/purchase-requisitions/{doku}` |
| **PurchaseOrders** | `GET/POST /api/purchase-orders`, `GET/PUT /api/purchase-orders/{doku}` |
| **GoodsReceipts** | `GET/POST /api/goods-receipts`, `GET/PUT /api/goods-receipts/{doku}` |
| **Invoices** | `GET/POST /api/invoices`, `GET/PUT /api/invoices/{doku}` |
| **Payments** | `GET/POST /api/payments`, `GET/PUT /api/payments/{doku}` |
| **Push** | `GET /api/push/vapid-public-key`, `POST /api/push/subscribe`, `DELETE /api/push/subscribe`, `POST /api/push/test` |

Status codes: 200 on success, 201 on create, 400 on validation,
401 on missing/invalid token, 404 on missing document, 409 on
idempotency-key reuse with a different body, 412 on stale ETag,
428 if `If-Match` is missing on PUT, 422 on business-rule violation.

## Cross-cutting behavior

### Auth: JWT + refresh token cookie

- Login: `POST /api/auth/login` with `{ username, password }`. On
  success returns `{ accessToken, expiresIn }` and sets an
  httpOnly `refreshToken` cookie (7-day expiry, `SameSite=Strict`,
  `Secure` when over HTTPS).
- Access token: HS256 JWT, 15-minute lifetime, signed with
  `Jwt:SecretKey`. Claims: `NameIdentifier` (userId), `Name` (username),
  `Role`. Send as `Authorization: Bearer <token>`.
- Refresh: `POST /api/auth/refresh` reads the cookie, rotates the
  stored refresh-token hash in `Master_Users`, and issues a new
  access token + cookie. The client (see [client README](../../galva-client/README.md))
  retries 401s automatically by calling refresh once.
- Logout: `POST /api/auth/logout` clears the cookie.

### Authorization

`Program.cs` registers a **fallback policy** that requires
authentication for every route by default. Endpoints that should be
anonymous call `.AllowAnonymous()` on the route
(`/api/auth/login`, `/api/auth/refresh`, `/api/auth/logout`).

### Idempotency

`IdempotencyMiddleware` intercepts POSTs that carry an
`Idempotency-Key` header:

1. Hashes `method + path + body` (SHA-256).
2. Looks up the key in `Tx_IdempotencyRecord`. If a record exists
   and the body hash matches, the cached response (status + body)
   is replayed.
3. If a record exists and the hash **doesn't** match, the response
   is `409 Conflict` (same key, different payload).
4. If no record exists (or the existing one is expired), the
   request runs through, and any 2xx response is persisted for
   24 hours.

The client SDK generates a UUID per `api.post()` call, so accidental
double-submits (button mashing, page reload during a pending POST)
are deduped automatically.

### ETag-based concurrency

Most read endpoints return an `ETag: "<base64>"` header. The base64
is the SQL Server `RowVersion` (a.k.a. `timestamp`) of the row.
Write endpoints (PUT) require the client to echo the ETag back as
`If-Match: <base64-or-quoted-base64>`. The handler:

1. Loads the entity.
2. Compares the supplied RowVersion to the current one with
   `SequenceEqual`.
3. If they differ, throws `ConcurrencyException` → handler maps
   to `412 Precondition Failed`.
4. If they match, applies the change and saves. EF Core
   `[Timestamp]`/`RowVersion` properties increment automatically
   on save, so the response includes a fresh ETag.

If the `If-Match` header is missing, the endpoint returns
`428 Precondition Required`.

### Validation

Every command has a sibling `*Validator` (FluentValidation). The
`ValidationBehaviour` MediatR pipeline runs all registered validators
before the handler. Failures are grouped by property name and
thrown as a `ValidationException`; the handler maps to
`400 Bad Request` with the field-error dictionary as the body.

## Configuration

`appsettings.json` is checked in. Sensitive values (JWT secret, VAPID
keys) must be in user-secrets, not in this file.

| Path | Purpose | Where |
|---|---|---|
| `ConnectionStrings:ErpApMockup` | SQL Server connection string | `appsettings.json` |
| `Jwt:Issuer` | JWT `iss` claim | `appsettings.json` |
| `Jwt:Audience` | JWT `aud` claim | `appsettings.json` |
| `Jwt:AccessTokenExpirationMinutes` | Access-token lifetime (default 15) | `appsettings.json` |
| `Jwt:RefreshTokenExpirationDays` | Refresh-cookie lifetime (default 7) | `appsettings.json` |
| `Jwt:SecretKey` | HS256 signing key (**must be ≥32 chars**) | user-secrets |
| `Cors:AllowedOrigins` | Allowed dev origins | `appsettings.json` |
| `Sentry:Dsn` | Sentry ingest URL (leave empty to disable) | `appsettings.json` |
| `Sentry:SendDefaultPii` | Whether to send user PII to Sentry | `appsettings.json` |
| `Sentry:TracesSampleRate` | APM sample rate 0.0-1.0 | `appsettings.json` |
| `VAPID:Subject` | Web push contact (mailto:) | `appsettings.json` |
| `VAPID:PublicKey` | VAPID public key | user-secrets |
| `VAPID:PrivateKey` | VAPID private key | user-secrets |

### Setting user-secrets

```bash
cd GalvaERP                                    # the .csproj folder
dotnet user-secrets init                       # one-time
dotnet user-secrets set "Jwt:SecretKey"        "REPLACE_ME_WITH_RANDOM_32_PLUS_CHARS"
dotnet user-secrets set "VAPID:PublicKey"      "BBu23JNyMfOPSIE4PNU2b3fxXA4wf2s17ubmExERj8Zg6rvMS-..."
dotnet user-secrets set "VAPID:PrivateKey"     "kAtGgkRTiLhX2TrV5dhTxpvZWje5TkB1jTNwSiLq6hE"
dotnet user-secrets list                       # verify
```

Generate a JWT secret with `openssl rand -base64 48`. VAPID keys
are generated with `web-push generate-vapid-keys` (from the
`web-push` npm package) or `npx web-push generate-vapid-keys`.

User-secrets land in `~/.microsoft/usersecrets/<UserSecretsId>/secrets.json`
on Linux/macOS — the `<UserSecretsId>` is `f225e738-2ff9-4f39-8928-9bf74dead68b`
(declared in `GalvaERP.csproj`).

### CORS

The `strict` policy in `Program.cs` allows origins from
`Cors:AllowedOrigins` (currently `http://localhost:5173` and
`http://localhost:5132`). To add a new dev port, append it to the
array in `appsettings.json`. The policy enables credentials
(needed for the refresh cookie) and is intentionally restrictive.

## Database

The API doesn't ship with a database. It expects the SQL Server
container from `../../galva-db/` to be running on
`localhost:1433` with database `ErpApMockup` already initialised
(`.NET 8`'s container `galva-mssql`).

On startup, EF Core opens the connection lazily — no migrations
are applied automatically. Schema changes need to come from
`../../galva-db/schema.sql` (or the seed-from-prod pipeline).

## Seeding the admin user

`Common/Security/AdminUserSeeder.cs` runs once on app startup. It
checks `Master_Users` for any existing user; if empty, it creates
`admin` / `admin123` with `Role=Admin` and a BCrypt-hashed password
(work factor 11). The seeder logs at `Information` level with the
default credentials — **change them in any non-dev environment**.

In production, gate the seeder behind a configuration flag or
remove it from `Program.cs`; never deploy with a default
`admin / admin123` in the database.

## Common tasks

**Add a new feature slice**

1. Create `Features/<Area>/Commands/CreateXCommand.cs` (a record
   implementing `IRequest<string>`) and a sibling
   `CreateXCommandHandler` that uses `AppDbContext` to persist.
2. Add `CreateXCommandValidator` for FluentValidation.
3. Add the DTOs in `Features/<Area>/DTOs/`.
4. Register the route in `Features/<Area>/<Area>Endpoints.cs` and
   call `app.Map<Area>Endpoints()` from `Program.cs`.
5. If the entity needs a new table, add it to `schema.sql` (or
   the seed-from-prod pipeline), add the entity class under
   `Domain/Entities/`, register the `DbSet` and fluent config in
   `Infrastructure/Data/AppDbContext.cs`.

**Run integration tests against the local DB**

Tests aren't set up yet. Recommended approach: a separate
`GalvaERP.IntegrationTests` xUnit project that spins up
`WebApplicationFactory<Program>` with the `Test` connection string
pointing at a dedicated test database. The DB container is already
up; just point at a different `Database=…` in the test config.

**Add a new master-data table**

1. `CREATE TABLE` it in `../../galva-db/schema.sql`.
2. Add the entity in `Domain/Entities/`.
3. Register the `DbSet<YourEntity>` and `entity.ToTable(...)` config
   in `AppDbContext.cs`.
4. Add a `GET /api/master-data/your-things` endpoint in
   `MasterDataEndpoints.cs` that returns a small projection
   (`Code`, `Nama`, etc.) — keep payloads small.
5. Add a typed query hook in the client (`useMasterData.ts`).

**Inspect the live API**

```bash
# While dotnet run is up:
curl http://localhost:5132/swagger/v1/swagger.json | jq
# or open http://localhost:5132/swagger in a browser
```

## Known quirks

- `CreatePurchaseOrderCommand` and `CreatePurchaseRequisitionCommand`
  **auto-generate** the `Doku` from the date prefix
  (`PO-YYYYMMDD-NNN`, `SPB-YYYYMMDD-NNN`). The `Doku` field on the
  request is ignored. This is intentional — clients can't pick
  arbitrary identifiers.
- The `STS` field on new POs is hardcoded to `"0"` (Pending). The
  client can transition it to `"1"` (Confirmed) or `"2"` (Cancelled)
  via PUT.
- The dev environment runs `dotnet run` over HTTP only. For HTTPS,
  use the `https` launch profile (requires `dotnet dev-certs https --trust`).
- `appsettings.json` is checked in; **never** put real secrets there.
  The JWT signing key, VAPID keys, and Sentry DSN all belong in
  user-secrets (dev) or environment variables (prod).
- A MediatR license-warning is logged on every startup
  (LuckyPennySoftware). It doesn't affect functionality; to silence
  it, set a license key in user-secrets or swap to the open-source
  `MediatR` package.

## Roadmap

- Integration tests with `WebApplicationFactory<Program>` +
  a dedicated test database.
- Pagination on list endpoints (currently unbounded `ToListAsync`).
- Filtering / search on list endpoints.
- Detail pages for PO/GR/Invoice on the client (the PR side is
  wired up; the others aren't yet).
- Web push: real production sender (the current `PushService` is
  a thin wrapper; needs a queue/retry layer for at-least-once
  delivery).
- A `/api/health` endpoint that pings the DB for container
  readiness probes.

## Related docs

- [Client README](../../galva-client/README.md) — what the SPA
  expects from each endpoint and how it handles auth/idempotency.
- [Database README](../../galva-db/README.md) — Docker setup,
  schema, and the seed-from-prod pipeline.
